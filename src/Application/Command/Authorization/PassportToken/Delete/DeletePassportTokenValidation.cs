using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;

namespace Application.Command.Authorization.PassportToken.Delete
{
    internal sealed class DeletePassportTokenValidation : IValidation<DeletePassportTokenCommand>
    {
        private readonly ITimeProvider prvTime;
        private readonly IPassportValidation srvValidation;

        private readonly IPassportTokenRepository repoToken;

        public DeletePassportTokenValidation(IPassportTokenRepository repoToken, IPassportValidation srvValidation, ITimeProvider prvTime)
        {
            this.prvTime = prvTime;
            this.srvValidation = srvValidation;

            this.repoToken = repoToken;
        }

        async ValueTask<IMessageResult<bool>> IValidation<DeletePassportTokenCommand>.ValidateAsync(DeletePassportTokenCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            srvValidation.ValidateGuid(msgMessage.PassportTokenId, "Passport token identifier");

            if (srvValidation.IsValid == true)
            {
                IRepositoryResult<bool> rsltToken = await repoToken.ExistsAsync(msgMessage.PassportTokenId, tknCancellation);

                rsltToken.Match(
                    msgError => srvValidation.Add(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                    bResult =>
                    {
                        if (bResult == false)
                            srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"Passport token {msgMessage.PassportTokenId} does not exist." });

                        return bResult;
                    });
            }

            return await Task.FromResult(
                    srvValidation.Match(
                        msgError => new MessageResult<bool>(msgError),
                        bResult => new MessageResult<bool>(bResult))
                    );
        }
    }
}