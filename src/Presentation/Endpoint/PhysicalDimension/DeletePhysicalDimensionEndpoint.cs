using Application.Command.PhysicalData.PhysicalDimension.Delete;
using Application.Interface.Result;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.PhysicalData
{
	public static class DeletePhysicalDimensionEndpoint
	{
		public const string Name = "DeletePhysicalDimension";

		public static void AddDeletePhysicalDimensionEndpoint(this IEndpointRouteBuilder epBuilder)
		{
			epBuilder.MapDelete(
				EndpointRoute.PhysicalDimension.Delete, DeletePhysicalDimension)
				.RequireAuthorization(EndpointAuthorization.Passport)
				.WithName(Name)
				.WithTags("PhysicalDimension")
				.Produces(StatusCodes.Status401Unauthorized)
				.Produces<bool>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		public static async Task<IResult> DeletePhysicalDimension(
			Guid guPhysicalDimensionId,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			Guid guPassportId = Guid.Empty;

			if (httpContext.TryParsePassportId(out guPassportId) == false)
				return Results.BadRequest("Passport could not be identified.");

			DeletePhysicalDimensionCommand cmdDelete = MapToCommand(guPassportId, guPhysicalDimensionId);

			IMessageResult<bool> mdtResult = await mdtMediator.Send(cmdDelete, tknCancellation);

				return mdtResult.Match(
					msgError => Results.BadRequest($"{msgError.Code}: {msgError.Description}"),
					bResult => Results.Ok(bResult));
		}

		private static DeletePhysicalDimensionCommand MapToCommand(Guid guPassportId, Guid guPhysicalDimensionId)
		{
			return new DeletePhysicalDimensionCommand()
			{
				RestrictedPassportId = guPassportId,
				PhysicalDimensionId = guPhysicalDimensionId
			};
		}
	}
}
