using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;

namespace Application.Command.Authorization.Passport.Update
{
    internal sealed class UpdatePassportValidation : IValidation<UpdatePassportCommand>
    {
        private readonly ITimeProvider prvTime;
        private readonly IPassportValidation srvValidation;

        private readonly IPassportRepository repoPassport;
        private readonly IPassportVisaRepository repoVisa;

        public UpdatePassportValidation(IPassportRepository repoPassport, IPassportVisaRepository repoVisa, IPassportValidation srvValidation, ITimeProvider prvTime)
        {
            this.prvTime = prvTime;
            this.srvValidation = srvValidation;

            this.repoPassport = repoPassport;
            this.repoVisa = repoVisa;
        }

        async ValueTask<IMessageResult<bool>> IValidation<UpdatePassportCommand>.ValidateAsync(UpdatePassportCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            if (DateTimeOffset.Compare(msgMessage.ExpiredAt, prvTime.GetUtcNow()) < 0)
                srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = "Expiration date must be in the future." });

            if (DateTimeOffset.Compare(msgMessage.LastCheckedAt, prvTime.GetUtcNow()) > 0)
                srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = "Last checked date must be in the past." });

            if (srvValidation.IsValid == true)
            {
                IRepositoryResult<bool> rsltPassport = await repoPassport.ExistsAsync(msgMessage.PassportIdToUpdate, tknCancellation);

                rsltPassport.Match(
                    msgError => srvValidation.Add(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                    bResult =>
                    {
                        if (bResult == false)
                            srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"Passport {msgMessage.PassportIdToUpdate} does not exist." });

                        return bResult;
                    });

                foreach (Guid guVisaId in msgMessage.PassportVisaId)
                {
                    IRepositoryResult<bool> rsltVisa = await repoVisa.ExistsAsync(guVisaId, tknCancellation);

                    rsltVisa.Match(
                        msgError => srvValidation.Add(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                        bResult =>
                        {
                            if (bResult == false)
                                srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"Visa {guVisaId} does not exists." });

                            return bResult;
                        });
                }
            }

            return await Task.FromResult(
                srvValidation.Match(
                    msgError => new MessageResult<bool>(msgError),
                    bResult => new MessageResult<bool>(bResult))
                );
        }
    }
}
