using Application.Command.PhysicalData.TimePeriod.Delete;
using Application.Common.Error;
using Application.Interface.Result;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.PhysicalData
{
	public static class DeleteTimePeriodEndpoint
	{
		public const string Name = "DeleteTimePeriod";

		public static void AddDeleteTimePeriodEndpoint(this IEndpointRouteBuilder epBuilder)
		{
			epBuilder.MapDelete(
				EndpointRoute.TimePeriod.Delete, DeleteTimePeriod)
				.RequireAuthorization(EndpointAuthorization.Passport)
				.WithName(Name)
				.WithTags("TimePeriod")
				.Produces(StatusCodes.Status401Unauthorized)
				.Produces(StatusCodes.Status403Forbidden)
				.Produces<bool>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		public static async Task<IResult> DeleteTimePeriod(
			Guid guTimePeriodId,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			Guid guPassportId = Guid.Empty;

			if (httpContext.TryParsePassportId(out guPassportId) == false)
				return Results.BadRequest("Passport could not be identified.");

			DeleteTimePeriodCommand cmdDelete = MapToCommand(guPassportId, guTimePeriodId);

			IMessageResult<bool> mdtResult = await mdtMediator.Send(cmdDelete, tknCancellation);

			return mdtResult.Match(
				msgError =>
				{
					if (msgError.Equals(AuthorizationError.PassportVisa.VisaDoesNotExist) == true)
						return Results.Forbid();

					return Results.BadRequest($"{msgError.Code}: {msgError.Description}");
				},
				bResult => Results.Ok(bResult));
		}

		private static DeleteTimePeriodCommand MapToCommand(Guid guPassportId, Guid guTimePeriodId)
		{
			return new DeleteTimePeriodCommand()
			{
				RestrictedPassportId = guPassportId,
				TimePeriodId = guTimePeriodId
			};
		}
	}
}
