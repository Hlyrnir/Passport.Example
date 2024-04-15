using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace Presentation.Authorization.Middleware
{
	public class AuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
	{
		private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

		public async Task HandleAsync(RequestDelegate dlgRequest, HttpContext httpContext, AuthorizationPolicy plcyAuthorization, PolicyAuthorizationResult plcyAuthorizationResult)
		{
			if (plcyAuthorizationResult.Forbidden
				&& plcyAuthorizationResult.AuthorizationFailure!.FailureReasons.Any())
			{
				httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
				await httpContext.Response.WriteAsJsonAsync(plcyAuthorizationResult.AuthorizationFailure!.FailureReasons);
				return;
			}

			// Fall back to the default implementation.
			await defaultHandler.HandleAsync(dlgRequest, httpContext, plcyAuthorization, plcyAuthorizationResult);
		}
	}
}
