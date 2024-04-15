using Application.Filter;
using Application.Interface.Result;
using Application.Query.PhysicalData.TimePeriod.ByFilter;
using Contract.v01.Request.PhysicalDimension;
using Contract.v01.Response.TimePeriod;
using Domain.Interface.PhysicalData;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.PhysicalData
{
	public static class FindTimePeriodByFilterEndpoint
	{
		public const string Name = "FindTimePeriodByFilter";

		public static void AddFindTimePeriodByFilterEndpoint(this IEndpointRouteBuilder epBuilder)
		{
			epBuilder.MapGet(
				EndpointRoute.TimePeriod.GetUnspecific, FindTimePeriodByFilter)
				.RequireAuthorization(EndpointAuthorization.Passport)
				.WithName(Name)
				.WithTags("TimePeriod")
				.Produces(StatusCodes.Status401Unauthorized)
				.Produces<TimePeriodByFilterResponse>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		private static async Task<IResult> FindTimePeriodByFilter(
			[AsParameters] FindTimePeriodByFilterRequest rqstTimePeriod,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			Guid guPassportId = Guid.Empty;

			if (httpContext.TryParsePassportId(out guPassportId) == false)
				return Results.BadRequest("Passport could not be identified.");

			TimePeriodByFilterQuery qryFindByFilter = rqstTimePeriod.MapToQuery(guPassportId);

			IMessageResult<TimePeriodByFilterResult> mdtResult = await mdtMediator.Send(qryFindByFilter, tknCancellation);

			return mdtResult.Match(
				msgError => Results.BadRequest($"{msgError.Code}: {msgError.Description}"),
				rsltTimePeriod => TypedResults.Ok(rsltTimePeriod.MapToResponse(
					qryFindByFilter.Filter.Page, 
					qryFindByFilter.Filter.PageSize)));
		}

		private static TimePeriodByFilterQuery MapToQuery(this FindTimePeriodByFilterRequest rqstTimePeriod, Guid guPassportId)
		{
			return new TimePeriodByFilterQuery()
			{
				RestrictedPassportId = guPassportId,
				Filter = new TimePeriodByFilterOption()
				{
					PhysicalDimensionId = rqstTimePeriod.PhysicalDimensionId,

					Page = rqstTimePeriod.Page,
					PageSize = rqstTimePeriod.PageSize
				}
			};
		}

		private static TimePeriodByFilterResponse MapToResponse(this TimePeriodByFilterResult rsltTimePeriod, int iPage, int iPageSize)
		{
			return new TimePeriodByFilterResponse()
			{
				TimePeriod = rsltTimePeriod.TimePeriod.MapToResponse(),
				Page = iPage,
				PageSize = iPageSize,
				ResultCount = rsltTimePeriod.MaximalNumberOfTimePeriod
			};
		}

		public static IEnumerable<TimePeriodByIdResponse> MapToResponse(this IEnumerable<ITimePeriod> enumTimePeriod)
		{
			foreach (ITimePeriod pdTimePeriod in enumTimePeriod)
			{
				yield return new TimePeriodByIdResponse()
				{
					Id = pdTimePeriod.Id,
					Magnitude = pdTimePeriod.Magnitude,
					Offset = pdTimePeriod.Offset,
					PhysicalDimensionId = pdTimePeriod.PhysicalDimensionId
				};
			}
		}
	}
}