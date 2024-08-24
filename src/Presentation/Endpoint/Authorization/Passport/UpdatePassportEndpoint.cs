using Application.Command.Authorization.Passport.Update;
using Application.Common.Error;
using Application.Interface.Result;
using Contract.v01.Request.Authorization.Passport;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.Authorization.Passport
{
    public static class UpdatePassportEndpoint
	{
		public const string Name = "UpdatePassport";

		public static void AddUpdatePassportEndpoint(this IEndpointRouteBuilder epBuilder)
		{
			epBuilder.MapPut(
				EndpointRoute.Passport.Update, UpdatePassport)
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

		public static async Task<IResult> UpdatePassport(
			UpdatePassportRequest rqstPassport,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			Guid guPassportId = Guid.Empty;

			if (httpContext.TryParsePassportId(out guPassportId) == false)
				return Results.BadRequest("Passport could not be identified.");

			UpdatePassportCommand cmdUpdate = rqstPassport.MapToCommand(guPassportId);

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

		private static UpdatePassportCommand MapToCommand(this UpdatePassportRequest cmdRequest, Guid guPassportId)
		{
			return new UpdatePassportCommand()
			{
				RestrictedPassportId = guPassportId,
				PassportIdToUpdate = cmdRequest.PassportId,
				ConcurrencyStamp = cmdRequest.ConcurrencyStamp,
				ExpiredAt = cmdRequest.ExpiredAt,
				IsAuthority = cmdRequest.IsAuthority,
				IsEnabled = cmdRequest.IsEnabled,
				LastCheckedAt = cmdRequest.LastCheckedAt,
				LastCheckedBy = cmdRequest.LastCheckedBy,
				PassportVisaId = cmdRequest.PassportVisa
			};
		}
	}
}