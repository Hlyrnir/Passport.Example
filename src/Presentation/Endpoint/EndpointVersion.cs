using Asp.Versioning;
using Asp.Versioning.Builder;

namespace Presentation.Endpoint
{
	public static class EndpointVersion
	{
		public static ApiVersionSet VersionSet { get; private set; }

		public static IEndpointRouteBuilder AddEndpointVersionSet(this IEndpointRouteBuilder epBuilder)
		{
			VersionSet = epBuilder.NewApiVersionSet()
				.HasApiVersion(new ApiVersion(1.0))
				.ReportApiVersions()
				.Build();

			return epBuilder;
		}
	}
}
