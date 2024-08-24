using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Interface.Result;
using Application.Interface.Validation;

namespace Application.Command.Authentication.BearerTokenByCredential
{
    internal sealed class BearerTokenByCredentialValidation : IValidation<BearerTokenByCredentialCommand>
	{
		private readonly IPassportValidation srvValidation;

		public BearerTokenByCredentialValidation(IPassportValidation srvValidation)
		{
			this.srvValidation = srvValidation;
		}

		async ValueTask<IMessageResult<bool>> IValidation<BearerTokenByCredentialCommand>.ValidateAsync(BearerTokenByCredentialCommand mdtCommand, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			srvValidation.ValidateProvider(mdtCommand.Credential.Provider, "Provider");
			srvValidation.ValidateCredential(mdtCommand.Credential.Credential, "Credential");
			srvValidation.ValidateSignature(mdtCommand.Credential.Signature, "Signature");

			return await Task.FromResult(
				srvValidation.Match(
					msgError => new MessageResult<bool>(msgError),
					bResult => new MessageResult<bool>(true))
				);
		}
	}
}
