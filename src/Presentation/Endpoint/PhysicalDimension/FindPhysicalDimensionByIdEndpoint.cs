using Application.Interface.Result;
using Application.Query.PhysicalData.PhysicalDimension.ById;
using Contract.v01.Response.PhysicalData;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.PhysicalData
{
	public static class FindPhysicalDimensionByIdEndpoint
	{
		public const string Name = "FindPhysicalDimensionById";

		public static void AddFindPhysicalDimensionByIdEndpoint(this IEndpointRouteBuilder epBuilder)
		{
			epBuilder.MapGet(
				EndpointRoute.PhysicalDimension.GetById, FindPhysicalDimensionById)
				.RequireAuthorization(EndpointAuthorization.Passport)
				.WithName(Name)
				.WithTags("PhysicalDimension")
				.Produces(StatusCodes.Status401Unauthorized)
				.Produces<PhysicalDimensionByIdResponse>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		public static async Task<IResult> FindPhysicalDimensionById(
			Guid guId,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			Guid guPassportId = Guid.Empty;

			if (httpContext.TryParsePassportId(out guPassportId) == false)
				return Results.BadRequest("Passport could not be identified.");

			PhysicalDimensionByIdQuery qryGetById = MapToQuery(guId, guPassportId);

			IMessageResult<PhysicalDimensionByIdResult> mdtResult = await mdtMediator.Send(qryGetById, tknCancellation);

			return mdtResult.Match(
				msgError => Results.BadRequest($"{msgError.Code}: {msgError.Description}"),
				rsltPhysicalDimension => TypedResults.Ok(rsltPhysicalDimension.MapToResponse()));
		}

		private static PhysicalDimensionByIdQuery MapToQuery(Guid guPhysicalDimensionId, Guid guPassportId)
		{
			return new PhysicalDimensionByIdQuery()
			{
				PhysicalDimensionId = guPhysicalDimensionId,
				RestrictedPassportId = guPassportId
			};
		}

		private static PhysicalDimensionByIdResponse MapToResponse(this PhysicalDimensionByIdResult rsltPhysicalDimension)
		{
			return new PhysicalDimensionByIdResponse()
			{
				ConversionFactorToSI = rsltPhysicalDimension.PhysicalDimension.ConversionFactorToSI,
				CultureName = rsltPhysicalDimension.PhysicalDimension.CultureName,
				ExponentOfAmpere = rsltPhysicalDimension.PhysicalDimension.ExponentOfUnit.Ampere,
				ExponentOfCandela = rsltPhysicalDimension.PhysicalDimension.ExponentOfUnit.Candela,
				ExponentOfKelvin = rsltPhysicalDimension.PhysicalDimension.ExponentOfUnit.Kelvin,
				ExponentOfKilogram = rsltPhysicalDimension.PhysicalDimension.ExponentOfUnit.Kilogram,
				ExponentOfMetre = rsltPhysicalDimension.PhysicalDimension.ExponentOfUnit.Metre,
				ExponentOfMole = rsltPhysicalDimension.PhysicalDimension.ExponentOfUnit.Mole,
				ExponentOfSecond = rsltPhysicalDimension.PhysicalDimension.ExponentOfUnit.Second,
				Id = rsltPhysicalDimension.PhysicalDimension.Id,
				Name = rsltPhysicalDimension.PhysicalDimension.Name,
				Symbol = rsltPhysicalDimension.PhysicalDimension.Symbol,
				Unit = rsltPhysicalDimension.PhysicalDimension.Unit
			};
		}
	}
}
