using Application.Command.PhysicalData.PhysicalDimension.Update;
using Application.Interface.Result;
using Contract.v01.Request.PhysicalDimension;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.PhysicalData
{
    public static class UpdatePhysicalDimensionEndpoint
	{
		public const string Name = "UpdatePhysicalDimension";

		public static void AddUpdatePhysicalDimensionEndpoint(this IEndpointRouteBuilder epBuilder)
		{
			epBuilder.MapPut(
				EndpointRoute.PhysicalDimension.Update, UpdatePhysicalDimension)
				.RequireAuthorization(EndpointAuthorization.Passport)
				.WithName(Name)
				.WithTags("PhysicalDimension")
				.Produces(StatusCodes.Status401Unauthorized)
				.Produces<bool>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		public static async Task<IResult> UpdatePhysicalDimension(
			UpdatePhysicalDimensionRequest rqstPhysicalDimension,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			Guid guPassportId = Guid.Empty;

			if (httpContext.TryParsePassportId(out guPassportId) == false)
				return Results.BadRequest("Passport could not be identified.");

			UpdatePhysicalDimensionCommand cmdUpdate = rqstPhysicalDimension.MapToCommand(guPassportId);

			IMessageResult<bool> mdtResult = await mdtMediator.Send(cmdUpdate, tknCancellation);

			return mdtResult.Match(
				msgError => Results.BadRequest($"{msgError.Code}: {msgError.Description}"),
				bResult => TypedResults.Ok(bResult));
		}

		private static UpdatePhysicalDimensionCommand MapToCommand(this UpdatePhysicalDimensionRequest cmdRequest, Guid guPassportId)
		{
			return new UpdatePhysicalDimensionCommand()
			{
				RestrictedPassportId = guPassportId,
				ConversionFactorToSI = cmdRequest.ConversionFactorToSI,
				CultureName = cmdRequest.CultureName,
				ExponentOfAmpere = cmdRequest.ExponentOfAmpere,
				ExponentOfCandela = cmdRequest.ExponentOfCandela,
				ExponentOfKelvin = cmdRequest.ExponentOfKelvin,
				ExponentOfKilogram = cmdRequest.ExponentOfKilogram,
				ExponentOfMetre = cmdRequest.ExponentOfMetre,
				ExponentOfMole = cmdRequest.ExponentOfMole,
				ExponentOfSecond = cmdRequest.ExponentOfSecond,
				PhysicalDimensionId = cmdRequest.PhysicalDimensionId,
				Name = cmdRequest.Name,
				Symbol = cmdRequest.Symbol,
				Unit = cmdRequest.Unit
			};
		}
	}
}
