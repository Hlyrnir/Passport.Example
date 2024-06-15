using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authorization.Passport.Update
{
    internal sealed class UpdatePassportCommandHandler : ICommandHandler<UpdatePassportCommand, IMessageResult<bool>>
    {
        private readonly ITimeProvider prvTime;
        private readonly IPassportRepository repoPassport;
        private readonly IPassportVisaRepository repoVisa;

        public UpdatePassportCommandHandler(
            ITimeProvider prvTime,
            IPassportRepository repoPassport,
            IPassportVisaRepository repoVisa)
        {
            this.prvTime = prvTime;
            this.repoPassport = repoPassport;
            this.repoVisa = repoVisa;
        }

        public async ValueTask<IMessageResult<bool>> Handle(UpdatePassportCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            IRepositoryResult<IPassport> rsltPassport = await repoPassport.FindByIdAsync(msgMessage.PassportIdToUpdate, tknCancellation);

            return await rsltPassport.MatchAsync(
                msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                async ppPassport =>
                {
                    if (ppPassport.ConcurrencyStamp != msgMessage.ConcurrencyStamp)
                        return new MessageResult<bool>(DefaultMessageError.ConcurrencyViolation);

                    if (msgMessage.IsAuthority == true || msgMessage.IsEnabled == true)
                    {
                        IRepositoryResult<IPassport> rsltAuthorizedPassport = await repoPassport.FindByIdAsync(msgMessage.RestrictedPassportId, tknCancellation);

                        IMessageResult<bool> rsltPassportIsChanged = rsltAuthorizedPassport.Match(
                            msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                            ppAuthority =>
                            {
                                if (ppPassport.IsAuthority == false && msgMessage.IsAuthority == true)
                                {
                                    if (ppPassport.TryJoinToAuthority(ppAuthority, prvTime.GetUtcNow()) == false)
                                        return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = $"Passport {ppPassport.Id} could not join to authority." });
                                }

                                if (ppPassport.IsEnabled == false && msgMessage.IsEnabled == true)
                                {
                                    if (ppPassport.TryEnable(ppAuthority, prvTime.GetUtcNow()) == false)
                                        return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = $"Passport {ppPassport.Id} is not enabled." });
                                }

                                return true;
                            });

                        if (rsltPassportIsChanged.IsFailed)
                            return rsltPassportIsChanged;
                    }

                    if (ppPassport.TryExtendTerm(msgMessage.ExpiredAt, prvTime.GetUtcNow(), msgMessage.RestrictedPassportId) == false)
                        return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = $"Term of passport {ppPassport.Id} could not be extended." });

                    foreach (Guid guPassportVisa in msgMessage.PassportVisaId)
                    {
                        IRepositoryResult<IPassportVisa> rsltVisa = await repoVisa.FindByIdAsync(guPassportVisa, tknCancellation);

                        IMessageResult<bool> rsltVisaIsAdded = rsltVisa.Match(
                            msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                            ppVisa =>
                            {
                                if (ppPassport.TryAddVisa(ppVisa) == false)
                                    return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = $"Visa {ppVisa} could not be added to passport {ppPassport.Id}" });

                                return true;
                            });

                        if (rsltVisaIsAdded.IsFailed)
                            return rsltVisaIsAdded;
                    }

                    IRepositoryResult<bool> rsltUpdate = await repoPassport.UpdateAsync(ppPassport, prvTime.GetUtcNow(), tknCancellation);

                    return rsltUpdate.Match(
                        msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                        bResult => new MessageResult<bool>(bResult));
                });
        }
    }
}