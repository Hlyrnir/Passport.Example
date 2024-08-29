using Application.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Authorization.Policy
{
    public static class EndpointPolicy
	{
		public static AuthorizationPolicy EndpointWithPassport()
		{
			var plcyBuilder = new AuthorizationPolicyBuilder();

			plcyBuilder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
			plcyBuilder.RequireAuthenticatedUser();
			plcyBuilder.RequireClaim(JwtTokenClaim.Id);

			return plcyBuilder.Build();
		}
	}
}
