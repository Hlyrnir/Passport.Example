using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authorization.PassportHolder.Update
{
    internal sealed class UpdatePassportHolderCommandHandler : ICommandHandler<UpdatePassportHolderCommand, IMessageResult<bool>>
    {
        private readonly ITimeProvider prvTime;
        private readonly IPassportHolderRepository repoHolder;
        private readonly IPassportSetting ppSetting;

        public UpdatePassportHolderCommandHandler(
            ITimeProvider prvTime,
            IPassportHolderRepository repoHolder,
            IPassportSetting ppSetting)
        {
            this.prvTime = prvTime;
            this.repoHolder = repoHolder;
            this.ppSetting = ppSetting;
        }

        public async ValueTask<IMessageResult<bool>> Handle(UpdatePassportHolderCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            IRepositoryResult<IPassportHolder> rsltHolder = await repoHolder.FindByIdAsync(msgMessage.PassportHolderId, tknCancellation);

            return await rsltHolder.MatchAsync(
                msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                async ppHolder =>
                {
                    if (ppHolder.ConcurrencyStamp != msgMessage.ConcurrencyStamp)
                        return new MessageResult<bool>(DefaultMessageError.ConcurrencyViolation);

                    if (ppHolder.CultureName != msgMessage.CultureName)
                    {
                        if (ppHolder.TryChangeCultureName(msgMessage.CultureName) == false)
                            return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = "Culture name could not be changed." });
                    }

                    if (ppHolder.EmailAddress != msgMessage.EmailAddress)
                    {
                        if (ppHolder.TryChangeEmailAddress(msgMessage.EmailAddress, ppSetting) == false)
                            return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = "Email address could not be changed." });
                    }

                    if (ppHolder.PhoneNumber != msgMessage.PhoneNumber)
                    {
                        if (ppHolder.TryChangePhoneNumber(msgMessage.PhoneNumber, ppSetting) == false)
                            return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = "Phone number could not be changed." });
                    }

                    if (ppHolder.FirstName != msgMessage.FirstName)
                        ppHolder.FirstName = msgMessage.FirstName;

                    if (ppHolder.LastName != msgMessage.LastName)
                        ppHolder.LastName = msgMessage.LastName;

                    IRepositoryResult<bool> rsltUpdate = await repoHolder.UpdateAsync(ppHolder, prvTime.GetUtcNow(), tknCancellation);

                    return rsltUpdate.Match(
                        msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                        bResult => new MessageResult<bool>(bResult));
                });
        }
    }
}