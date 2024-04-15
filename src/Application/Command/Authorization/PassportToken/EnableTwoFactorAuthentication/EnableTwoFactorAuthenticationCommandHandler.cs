using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authorization.PassportToken.EnableTwoFactorAuthentication
{
	internal sealed class EnableTwoFactorAuthenticationCommandHandler : ICommandHandler<EnableTwoFactorAuthenticationCommand, IMessageResult<bool>>
	{
		private readonly ITimeProvider prvTime;
		private readonly IPassportTokenRepository repoToken;

		public EnableTwoFactorAuthenticationCommandHandler(
			ITimeProvider prvTime,
			IPassportTokenRepository repoToken)
		{
			this.prvTime = prvTime;
			this.repoToken = repoToken;
		}

		public async ValueTask<IMessageResult<bool>> Handle(EnableTwoFactorAuthenticationCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			IRepositoryResult<IPassportToken> rsltToken = await repoToken.FindTokenByCredentialAsync(msgMessage.CredentialToVerify, prvTime.GetUtcNow(), tknCancellation);

			return await rsltToken.MatchAsync(
				msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				async ppToken =>
				{
					if (ppToken.TwoFactorIsEnabled == true)
						return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = "Two factor authentication is already enabled." });

					IRepositoryResult<bool> rsltEnable = await repoToken.EnableTwoFactorAuthenticationAsync(ppToken, msgMessage.TwoFactorIsEnabled, prvTime.GetUtcNow(), tknCancellation);

					return rsltEnable.Match(
						msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
						bResult => new MessageResult<bool>(bResult));
				});
		}
	}
}