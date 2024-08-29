using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Interface.Result;
using Application.Interface.Validation;

namespace Application.Command.Authentication.JwtTokenByRefreshToken
{
	internal sealed class JwtTokenByRefreshTokenValidation : IValidation<JwtTokenByRefreshTokenCommand>
	{
		private readonly IPassportValidation srvValidation;

		public JwtTokenByRefreshTokenValidation(IPassportValidation srvValidation)
		{
			this.srvValidation = srvValidation;
		}

		async ValueTask<IMessageResult<bool>> IValidation<JwtTokenByRefreshTokenCommand>.ValidateAsync(JwtTokenByRefreshTokenCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			srvValidation.ValidateGuid(msgMessage.PassportId, "Id");
			srvValidation.ValidateProvider(msgMessage.Provider, "Provider");

			if (string.IsNullOrWhiteSpace(msgMessage.RefreshToken) == true)
				srvValidation.Add(ValidationError.Passport.InvalidRefreshToken);

			return await Task.FromResult(
				srvValidation.Match(
					msgError => new MessageResult<bool>(msgError),
					bResult => new MessageResult<bool>(true))
				);
		}
	}
}
