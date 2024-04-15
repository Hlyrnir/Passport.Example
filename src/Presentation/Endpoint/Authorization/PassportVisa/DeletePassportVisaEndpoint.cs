using Application.Command.Authorization.PassportVisa.Delete;
using Application.Interface.Result;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.Authorization.PassportVisa
{
	public static class DeletePassportVisaEndpoint
	{
		public const string Name = "DeletePassportVisa";

		public static void AddDeletePassportVisaEndpoint(this IEndpointRouteBuilder epBuilder)
		{
			epBuilder.MapDelete(
				EndpointRoute.PassportVisa.Delete, DeletePassportVisa)
				.RequireAuthorization(EndpointAuthorization.Passport)
				.WithName(Name)
				.WithTags("PassportVisa")
				.Produces(StatusCodes.Status401Unauthorized)
				.Produces<bool>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		public static async Task<IResult> DeletePassportVisa(
			Guid guPassportVisaId,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			Guid guPassportId = Guid.Empty;

			if (httpContext.TryParsePassportId(out guPassportId) == false)
				return Results.BadRequest("Passport could not be identified.");

			DeletePassportVisaCommand cmdUpdate = MapToCommand(guPassportId, guPassportVisaId);

			IMessageResult<bool> mdtResult = await mdtMediator.Send(cmdUpdate, tknCancellation);

			return mdtResult.Match(
				msgError => Results.BadRequest($"{msgError.Code}: {msgError.Description}"),
				bResult => TypedResults.Ok(bResult));
		}

		private static DeletePassportVisaCommand MapToCommand(Guid guPassportId, Guid guPassportVisaId)
		{
			return new DeletePassportVisaCommand()
			{
				RestrictedPassportId = guPassportId,
				PassportVisaId = guPassportVisaId
			};
		}
	}
}
