using Application.Command.Authorization.PassportVisa.Update;
using Application.Interface.Result;
using Contract.v01.Request.Authorization.PassportVisa;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.Authorization.PassportVisa
{
	public static class UpdatePassportVisaEndpoint
	{
		public const string Name = "UpdatePassportVisa";

		public static void AddUpdatePassportVisaEndpoint(this IEndpointRouteBuilder epBuilder)
		{
			epBuilder.MapPut(
				EndpointRoute.PassportVisa.Update, UpdatePassportVisa)
				.RequireAuthorization(EndpointAuthorization.Passport)
				.WithName(Name)
				.WithTags("PassportVisa")
				.Produces(StatusCodes.Status401Unauthorized)
				.Produces<bool>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		public static async Task<IResult> UpdatePassportVisa(
			UpdatePassportVisaRequest rqstPassportVisa,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			Guid guPassportId = Guid.Empty;

			if (httpContext.TryParsePassportId(out guPassportId) == false)
				return Results.BadRequest("Passport could not be identified.");

			UpdatePassportVisaCommand cmdUpdate = rqstPassportVisa.MapToCommand(guPassportId);

			IMessageResult<bool> mdtResult = await mdtMediator.Send(cmdUpdate, tknCancellation);

			return mdtResult.Match(
				msgError => Results.BadRequest($"{msgError.Code}: {msgError.Description}"),
				bResult => TypedResults.Ok(bResult));
		}

		private static UpdatePassportVisaCommand MapToCommand(this UpdatePassportVisaRequest rqstPassportVisa, Guid guPassportId)
		{
			return new UpdatePassportVisaCommand()
			{
				RestrictedPassportId = guPassportId,
				PassportVisaId = rqstPassportVisa.Id,
				ConcurrencyStamp = rqstPassportVisa.ConcurrencyStamp,
				Name = rqstPassportVisa.Name,
				Level = rqstPassportVisa.Level
			};
		}
	}
}