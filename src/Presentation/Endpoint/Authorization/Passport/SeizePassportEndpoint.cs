using Application.Command.Authorization.Passport.Seize;
using Application.Common.Error;
using Application.Interface.Result;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.Authorization.Passport
{
    public static class SeizePassportEndpoint
	{
		public const string Name = "SeizePassport";

		public static void AddSeizePassportEndpoint(this IEndpointRouteBuilder epBuilder)
		{
			epBuilder.MapDelete(
				EndpointRoute.Passport.Delete, SeizePassport)
				.RequireAuthorization(EndpointAuthorization.Passport)
				.WithName(Name)
				.WithTags("Passport")
				.Produces(StatusCodes.Status401Unauthorized)
				.Produces(StatusCodes.Status403Forbidden)
				.Produces<bool>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		public static async Task<IResult> SeizePassport(
			Guid guPassportVisaId,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			Guid guPassportId = Guid.Empty;

			if (httpContext.TryParsePassportId(out guPassportId) == false)
				return Results.BadRequest("Passport could not be identified.");

			SeizePassportCommand cmdUpdate = MapToCommand(guPassportId, guPassportVisaId);

			IMessageResult<bool> mdtResult = await mdtMediator.Send(cmdUpdate, tknCancellation);

			return mdtResult.Match(
				msgError =>
				{
					if (msgError.Equals(AuthorizationError.PassportVisa.VisaDoesNotExist) == true)
						return Results.Forbid();

					return Results.BadRequest($"{msgError.Code}: {msgError.Description}");
				},
				bResult => TypedResults.Ok(bResult));
		}

		private static SeizePassportCommand MapToCommand(Guid guPassportId, Guid guPassportToSeizeId)
		{
			return new SeizePassportCommand()
			{
				RestrictedPassportId = guPassportId,
				PassportIdToSeize = guPassportToSeizeId
			};
		}
	}
}