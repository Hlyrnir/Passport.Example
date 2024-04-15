using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;

namespace Application.Command.Authorization.PassportToken.EnableTwoFactorAuthentication
{
	internal sealed class EnableTwoFactorAuthenticationValidation : IValidation<EnableTwoFactorAuthenticationCommand>
	{
		private readonly ITimeProvider prvTime;
		private readonly IPassportValidation srvValidation;

		private readonly IPassportTokenRepository repoToken;

		public EnableTwoFactorAuthenticationValidation(IPassportTokenRepository repoToken, IPassportValidation srvValidation, ITimeProvider prvTime)
		{
			this.prvTime = prvTime;
			this.srvValidation = srvValidation;

			this.repoToken = repoToken;
		}

		async ValueTask<IMessageResult<bool>> IValidation<EnableTwoFactorAuthenticationCommand>.ValidateAsync(EnableTwoFactorAuthenticationCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			srvValidation.ValidateCredential(msgMessage.CredentialToVerify.Credential, "Credential");
			srvValidation.ValidateProvider(msgMessage.CredentialToVerify.Provider, "Provider");
			srvValidation.ValidateSignature(msgMessage.CredentialToVerify.Signature, "Signature");

			if (srvValidation.IsValid == true)
			{
				IRepositoryResult<bool> rsltToken = await repoToken.CredentialAtProviderExistsAsync(
					sCredential: msgMessage.CredentialToVerify.Credential,
					sProvider: msgMessage.CredentialToVerify.Provider,
					tknCancellation: tknCancellation);

				rsltToken.Match(
					msgError => srvValidation.Add(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
					bResult =>
					{
						if (bResult == false)
							srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"Credential {msgMessage.CredentialToVerify.Credential} does not exist." });

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