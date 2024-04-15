using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;

namespace Application.Command.Authorization.PassportToken.ResetCredential
{
	internal sealed class ResetCredentialValidation : IValidation<ResetCredentialCommand>
	{
		private readonly ITimeProvider prvTime;
		private readonly IPassportValidation srvValidation;

		private readonly IPassportTokenRepository repoToken;

		public ResetCredentialValidation(IPassportTokenRepository repoToken, IPassportValidation srvValidation, ITimeProvider prvTime)
		{
			this.prvTime = prvTime;
			this.srvValidation = srvValidation;

			this.repoToken = repoToken;
		}

		async ValueTask<IMessageResult<bool>> IValidation<ResetCredentialCommand>.ValidateAsync(ResetCredentialCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			srvValidation.ValidateProvider(msgMessage.CredentialToVerify.Provider, "Provider");

			if (msgMessage.CredentialToApply.Provider != msgMessage.CredentialToVerify.Provider)
				srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = "Provider must be identical." });

			srvValidation.ValidateCredential(msgMessage.CredentialToApply.Credential, "Credential to apply");
			srvValidation.ValidateSignature(msgMessage.CredentialToApply.Signature, "Signature to apply");

			srvValidation.ValidateCredential(msgMessage.CredentialToVerify.Credential, "Credential to reset");
			srvValidation.ValidateSignature(msgMessage.CredentialToVerify.Signature, "Signature to reset");

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
