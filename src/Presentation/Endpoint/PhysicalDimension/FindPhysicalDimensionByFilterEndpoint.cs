using Application.Filter;
using Application.Interface.Result;
using Application.Query.PhysicalData.PhysicalDimension.ByFilter;
using Contract.v01.Request.PhysicalDimension;
using Contract.v01.Response.PhysicalDimension;
using Domain.Interface.PhysicalData;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.PhysicalData
{
	public static class FindPhysicalDimensionByFilterEndpoint
	{
		public const string Name = "FindPhysicalDimensionByFilter";

		public static void AddFindPhysicalDimensionByFilterEndpoint(this IEndpointRouteBuilder epBuilder)
		{
			epBuilder.MapGet(
				EndpointRoute.PhysicalDimension.GetUnspecific, FindPhysicalDimensionByFilter)
				.RequireAuthorization(EndpointAuthorization.Passport)
				.WithName(Name)
				.WithTags("PhysicalDimension")
				.Produces(StatusCodes.Status401Unauthorized)
				.Produces<PhysicalDimensionByFilterResponse>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		private static async Task<IResult> FindPhysicalDimensionByFilter(
			[AsParameters] FindPhysicalDimensionByFilterRequest rqstPhysicalDimension,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			Guid guPassportId = Guid.Empty;

			if (httpContext.TryParsePassportId(out guPassportId) == false)
				return Results.BadRequest("Passport could not be identified.");

			PhysicalDimensionByFilterQuery qryFindByFilter = rqstPhysicalDimension.MapToQuery(guPassportId);

			IMessageResult<PhysicalDimensionByFilterResult> mdtResult = await mdtMediator.Send(qryFindByFilter, tknCancellation);

			return mdtResult.Match(
				msgError => Results.BadRequest($"{msgError.Code}: {msgError.Description}"),
				rsltPhysicalDimension => TypedResults.Ok(rsltPhysicalDimension.MapToResponse(
					iPage: qryFindByFilter.Filter.Page,
					iPageSize: qryFindByFilter.Filter.PageSize)));
		}

		private static PhysicalDimensionByFilterQuery MapToQuery(this FindPhysicalDimensionByFilterRequest rqstPhysicalDimension, Guid guPassportId)
		{
			return new PhysicalDimensionByFilterQuery()
			{
				RestrictedPassportId = guPassportId,
				Filter = new PhysicalDimensionByFilterOption
				{
					ConversionFactorToSI = rqstPhysicalDimension.ConversionFactorToSI,
					CultureName = rqstPhysicalDimension.CultureName,
					ExponentOfAmpere = rqstPhysicalDimension.ExponentOfAmpere,
					ExponentOfCandela = rqstPhysicalDimension.ExponentOfCandela,
					ExponentOfKelvin = rqstPhysicalDimension.ExponentOfKelvin,
					ExponentOfKilogram = rqstPhysicalDimension.ExponentOfKilogram,
					ExponentOfMetre = rqstPhysicalDimension.ExponentOfMetre,
					ExponentOfMole = rqstPhysicalDimension.ExponentOfMole,
					ExponentOfSecond = rqstPhysicalDimension.ExponentOfSecond,
					Name = rqstPhysicalDimension.Name,
					Symbol = rqstPhysicalDimension.Symbol,
					Unit = rqstPhysicalDimension.Unit,

					Page = rqstPhysicalDimension.Page,
					PageSize = rqstPhysicalDimension.PageSize
				}
			};
		}

		private static PhysicalDimensionByFilterResponse MapToResponse(this PhysicalDimensionByFilterResult rsltPhysicalDimension, int iPage, int iPageSize)
		{
			return new PhysicalDimensionByFilterResponse()
			{
				PhysicalDimension = rsltPhysicalDimension.PhysicalDimension.MapToResponse(),
				Page = iPage,
				PageSize = iPageSize,
				ResultCount = rsltPhysicalDimension.MaximalNumberOfPhysicalDimension
			};
		}

		public static IEnumerable<PhysicalDimensionByIdResponse> MapToResponse(this IEnumerable<IPhysicalDimension> enumPhysicalDimension)
		{
			foreach (IPhysicalDimension pdPhysicalDimension in enumPhysicalDimension)
			{
				yield return new PhysicalDimensionByIdResponse()
				{
					ConcurrencyStamp = pdPhysicalDimension.ConcurrencyStamp,
					ConversionFactorToSI = pdPhysicalDimension.ConversionFactorToSI,
					CultureName = pdPhysicalDimension.CultureName,
					ExponentOfAmpere = pdPhysicalDimension.ExponentOfUnit.Ampere,
					ExponentOfCandela = pdPhysicalDimension.ExponentOfUnit.Candela,
					ExponentOfKelvin = pdPhysicalDimension.ExponentOfUnit.Kelvin,
					ExponentOfKilogram = pdPhysicalDimension.ExponentOfUnit.Kilogram,
					ExponentOfMetre = pdPhysicalDimension.ExponentOfUnit.Metre,
					ExponentOfMole = pdPhysicalDimension.ExponentOfUnit.Mole,
					ExponentOfSecond = pdPhysicalDimension.ExponentOfUnit.Second,
					Id = pdPhysicalDimension.Id,
					Name = pdPhysicalDimension.Name,
					Symbol = pdPhysicalDimension.Symbol,
					Unit = pdPhysicalDimension.Unit
				};
			}
		}
	}
}