using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Presentation.Authentication
{
	public class ConfigureAuthenticationOption : IConfigureOptions<AuthenticationOptions>
	{
		public void Configure(AuthenticationOptions optAuthentication)
		{
			optAuthentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			optAuthentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			optAuthentication.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
		}
	}

	public class ConfigureJwtBearerOption : IConfigureOptions<JwtBearerOptions>
	{
		private readonly IConfiguration cfgConfiguration;

		public ConfigureJwtBearerOption(IConfiguration cfgConfiguration)
		{
			this.cfgConfiguration = cfgConfiguration;
		}

		public void Configure(JwtBearerOptions optJwtBearer)
		{
			optJwtBearer.RequireHttpsMetadata = true;
			optJwtBearer.TokenValidationParameters = new TokenValidationParameters
			{
				ClockSkew = TimeSpan.FromSeconds(15),

				NameClaimType = ClaimTypes.NameIdentifier,
				ValidIssuer = cfgConfiguration["JwtSettings:Issuer"],
				ValidAudience = cfgConfiguration["JwtSettings:Audience"],
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfgConfiguration["JwtSettings:SecretKey"])),

				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true
			};
		}
	}
}
