using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;

namespace Application.Command.Authorization.Passport.Register
{
    internal sealed class RegisterPassportValidation : IValidation<RegisterPassportCommand>
    {
        private readonly ITimeProvider prvTime;
        private readonly IPassportValidation srvValidation;

        private readonly IPassportTokenRepository repoToken;

        public RegisterPassportValidation(IPassportTokenRepository repoToken, IPassportValidation srvValidation, ITimeProvider prvTime)
        {
            this.prvTime = prvTime;
            this.srvValidation = srvValidation;

            this.repoToken = repoToken;
        }

        async ValueTask<IMessageResult<bool>> IValidation<RegisterPassportCommand>.ValidateAsync(RegisterPassportCommand msgMessage, CancellationToken tknCancellation)
        {
            if (tknCancellation.IsCancellationRequested)
                return new MessageResult<bool>(DefaultMessageError.TaskAborted);

            srvValidation.ValidateCredential(msgMessage.CredentialToRegister.Credential, "Credential");
            srvValidation.ValidateProvider(msgMessage.CredentialToRegister.Provider, "Provider");
            srvValidation.ValidateSignature(msgMessage.CredentialToRegister.Signature, "Signature");

            srvValidation.ValidateEmailAddress(msgMessage.EmailAddress, "E-mail address");
            srvValidation.ValidatePhoneNumber(msgMessage.PhoneNumber, "Phone number");

            if (srvValidation.IsValid == true)
            {
                IRepositoryResult<bool> rsltCredential = await repoToken.CredentialAtProviderExistsAsync(
                    sCredential: msgMessage.CredentialToRegister.Credential,
                    sProvider: msgMessage.CredentialToRegister.Provider,
                    tknCancellation: tknCancellation);

                rsltCredential.Match(
                    msgError => srvValidation.Add(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
                    bResult =>
                    {
                        if (bResult == true)
                            srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"Credential {msgMessage.CredentialToRegister.Credential} does already exist." });

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
