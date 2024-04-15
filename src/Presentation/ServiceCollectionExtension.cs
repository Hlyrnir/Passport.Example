using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Presentation.Authorization.Middleware;

namespace Presentation
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddPresentation(this IServiceCollection cltService)
		{
			// Add authorization result handler
			cltService.TryAddScoped<IAuthorizationMiddlewareResultHandler, AuthorizationResultHandler>();

			return cltService;
		}
	}
}
