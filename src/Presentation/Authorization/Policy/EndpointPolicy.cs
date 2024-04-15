using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Presentation.Authorization.Policy
{
    public static class EndpointPolicy
	{
		public static AuthorizationPolicy EndpointWithPassport()
		{
			var plcyBuilder = new AuthorizationPolicyBuilder();

			plcyBuilder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
			plcyBuilder.RequireAuthenticatedUser();
			plcyBuilder.RequireClaim(ClaimTypes.NameIdentifier);

			return plcyBuilder.Build();
		}
	}
}
