using Application.Command.Authorization.PassportVisa.Create;
using Application.Interface.Result;
using Contract.v01.Request.Authorization.PassportVisa;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.Authorization.PassportVisa
{
	public static class CreatePassportVisaEndpoint
	{
		public const string Name = "CreatePassportVisa";

		public static void AddCreatePassportVisaEndpoint(this IEndpointRouteBuilder epBuilder)
		{
			epBuilder.MapPost(
				EndpointRoute.PassportVisa.Create, CreatePassportVisa)
				.RequireAuthorization(EndpointAuthorization.Passport)
				.WithName(Name)
				.WithTags("PassportVisa")
				.Produces(StatusCodes.Status401Unauthorized)
				.Produces(StatusCodes.Status201Created)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		public static async Task<IResult> CreatePassportVisa(
			CreatePassportVisaRequest rqstPassportVisa,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			Guid guPassportId = Guid.Empty;

			if (httpContext.TryParsePassportId(out guPassportId) == false)
				return Results.BadRequest("Passport could not be identified.");

			CreatePassportVisaCommand cmdInsert = rqstPassportVisa.MapToCommand(guPassportId);

			IMessageResult<Guid> mdtResult = await mdtMediator.Send(cmdInsert, tknCancellation);

			return mdtResult.Match(
				msgError => Results.BadRequest($"{msgError.Code}: {msgError.Description}"),
				guPassportVisaId => TypedResults.CreatedAtRoute(FindPassportVisaByIdEndpoint.Name, new { guId = guPassportVisaId }));
		}

		private static CreatePassportVisaCommand MapToCommand(this CreatePassportVisaRequest cmdRequest, Guid guPassportId)
		{
			return new CreatePassportVisaCommand()
			{
				RestrictedPassportId = guPassportId,
				Name = cmdRequest.Name,
				Level = cmdRequest.Level
			};
		}
	}
}
