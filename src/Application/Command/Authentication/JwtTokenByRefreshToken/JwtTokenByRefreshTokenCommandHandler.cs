using Application.Common.Authentication;
using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Common.Result.Repository;
using Application.Interface.Authentication;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Token;
using Domain.Interface.Authorization;
using Mediator;
using Microsoft.Extensions.Options;

namespace Application.Command.Authentication.JwtTokenByRefreshToken
{
	internal sealed class JwtTokenByRefreshTokenCommandHandler : ICommandHandler<JwtTokenByRefreshTokenCommand, IMessageResult<JwtTokenTransferObject>>
	{
		private readonly ITimeProvider prvTime;

		private readonly IJwtTokenSetting jwtSetting;
		private readonly IPassportRepository repoPassport;
		private readonly IPassportTokenRepository repoToken;

		private readonly IJwtTokenService jwtTokenService;

		public JwtTokenByRefreshTokenCommandHandler(
			ITimeProvider prvTime,
			IPassportRepository repoPassport,
			IPassportTokenRepository repoToken,
			IJwtTokenService jwtTokenService,
			IOptions<JwtTokenSetting> optJwtToken)
		{
			this.prvTime = prvTime;


			this.repoPassport = repoPassport;
			this.repoToken = repoToken;

			this.jwtTokenService = jwtTokenService;
			jwtSetting = optJwtToken.Value;
		}

		public async ValueTask<IMessageResult<JwtTokenTransferObject>> Handle(JwtTokenByRefreshTokenCommand cmdToken, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<JwtTokenTransferObject>(DefaultMessageError.TaskAborted);

			RepositoryResult<IPassportToken> pprToken = await repoToken.FindTokenByRefreshTokenAsync(cmdToken.PassportId, cmdToken.Provider, cmdToken.RefreshToken, prvTime.GetUtcNow(), tknCancellation);

			return await pprToken.MatchAsync(
				msgError => new MessageResult<JwtTokenTransferObject>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				async ppToken =>
				{
					IRepositoryResult<IPassport> rsltPassport = await repoPassport.FindByIdAsync(ppToken.PassportId, tknCancellation);

					return rsltPassport.Match(
						msgError => new MessageResult<JwtTokenTransferObject>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
						ppPassport =>
						{
							if (ppPassport.IsEnabled == false)
								return new MessageResult<JwtTokenTransferObject>(AuthorizationError.Passport.IsDisabled);

							return jwtTokenService.Generate(ppPassport, ppToken);
						});
				});
		}
	}
}