using Application.Common.Authentication;
using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Common.Result.Repository;
using Application.Interface.Authentication;
using Application.Interface.Passport;
using Application.Interface.Result;
using Application.Interface.Time;
using Domain.Interface.Authorization;
using Mediator;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Command.Authentication.BearerTokenByRefreshToken
{
    internal sealed class BearerTokenByRefreshTokenCommandHandler : ICommandHandler<BearerTokenByRefreshTokenCommand, IMessageResult<string>>
	{
		private readonly ITimeProvider prvTime;

		private readonly IJwtTokenSetting jwtSetting;
		private readonly IPassportRepository repoPassport;
		private readonly IPassportTokenRepository repoToken;

		public BearerTokenByRefreshTokenCommandHandler(
			ITimeProvider prvTime,
			IPassportRepository repoPassport,
			IPassportTokenRepository repoToken,
			IOptions<JwtTokenSetting> optJwtToken)
		{
			this.prvTime = prvTime;

			jwtSetting = optJwtToken.Value;
			this.repoPassport = repoPassport;
			this.repoToken = repoToken;
		}

		public async ValueTask<IMessageResult<string>> Handle(BearerTokenByRefreshTokenCommand cmdToken, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<string>(DefaultMessageError.TaskAborted);

			RepositoryResult<IPassportToken> pprToken = await repoToken.FindTokenByRefreshTokenAsync(cmdToken.PassportId, cmdToken.Provider, cmdToken.RefreshToken, prvTime.GetUtcNow(), tknCancellation);

			return await pprToken.MatchAsync(
				msgError => new MessageResult<string>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				async ppToken =>
				{
					IRepositoryResult<IPassport> rsltPassport = await repoPassport.FindByIdAsync(ppToken.PassportId, tknCancellation);

					return rsltPassport.Match(
						msgError => new MessageResult<string>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
						ppPassport =>
						{
							IEnumerable<Claim> lstClaim = ppToken.GenerateClaim(ppPassport, prvTime.GetUtcNow());

							if (lstClaim.IsNullOrEmpty() == true)
								return new MessageResult<string>(new MessageError() { Code = AuthorizationError.Code.Method, Description = "Passport is not enabled or expired." });

							SecurityTokenDescriptor tknDescriptor = new SecurityTokenDescriptor
							{
								Subject = new ClaimsIdentity(lstClaim, jwtSetting.Type),
								IssuedAt = prvTime.GetUtcNow().UtcDateTime,
								Expires = prvTime.GetUtcNow().AddMinutes(5).UtcDateTime,
								NotBefore = prvTime.GetUtcNow().UtcDateTime,
								Issuer = jwtSetting.Issuer,
								Audience = jwtSetting.Audience,
								SigningCredentials = new SigningCredentials(
												new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.SecretKey)),
												SecurityAlgorithms.HmacSha256Signature)
							};

							JwtSecurityTokenHandler tknHandler = new JwtSecurityTokenHandler();
							SecurityToken tknToken = tknHandler.CreateToken(tknDescriptor);
							string sToken = tknHandler.WriteToken(tknToken);

							return new MessageResult<string>(sToken);
						});
				});
		}
	}
}