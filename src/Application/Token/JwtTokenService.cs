using Application.Common.Authentication;
using Application.Interface.Authentication;
using Application.Interface.Time;
using Domain.Interface.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Application.Token
{
    internal sealed class JwtTokenService : IJwtTokenService
	{
		private readonly ITimeProvider prvTime;

		private readonly IJwtTokenSetting jwtSetting;

		public JwtTokenService(ITimeProvider prvTime, IOptions<JwtTokenSetting> optJwtToken)
		{
			this.prvTime = prvTime;

			this.jwtSetting = optJwtToken.Value;
		}

		public JwtTokenTransferObject Generate(IPassport ppPassport, IPassportToken ppToken)
		{
			IDictionary<string, object> dictClaim = new Dictionary<string, object>();

			dictClaim.Add(new KeyValuePair<string, object>(JwtTokenClaim.Id, ppPassport.Id));

			SecurityTokenDescriptor tknDescriptor = new SecurityTokenDescriptor
			{
				Claims = dictClaim,
				IssuedAt = prvTime.GetUtcNow().UtcDateTime,
				Expires = prvTime.GetUtcNow().AddMinutes(jwtSetting.LifetimeInMinutes).UtcDateTime,
				NotBefore = prvTime.GetUtcNow().UtcDateTime,
				Issuer = jwtSetting.Issuer,
				Audience = jwtSetting.Audience,
				SigningCredentials = new SigningCredentials(
														new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.SecretKey)),
														SecurityAlgorithms.HmacSha256Signature)
			};

			JwtSecurityTokenHandler tknHandler = new JwtSecurityTokenHandler();
			SecurityToken tknToken = tknHandler.CreateToken(tknDescriptor);

			return new JwtTokenTransferObject()
			{
				ExpiredAt = ppPassport.ExpiredAt,
				Provider = ppToken.Provider,
				RefreshToken = ppToken.RefreshToken,
				Token = tknHandler.WriteToken(tknToken)
			};
		}
	}
}
