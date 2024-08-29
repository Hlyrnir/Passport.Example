using Application.Interface.Authentication;

namespace Application.Common.Authentication
{
    public class JwtTokenSetting : IJwtTokenSetting
	{
		public static string SectionName = "JwtSetting";

		public string Type { get; } = "jwtAuthentication";
		public string Audience { get; init; } = string.Empty;
		public string Issuer { get; init; } = string.Empty;
		public string SecretKey { get; init; } = string.Empty;

		public int LifetimeInMinutes { get; init; } = 5;
	}
}
