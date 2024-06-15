using Application.Interface.Result;
using Application.Query.Authorization.PassportVisa.ById;
using Contract.v01.Response.Passport;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.Authorization.PassportVisa
{
    public static class FindPassportVisaByIdEndpoint
	{
		public const string Name = "FindPassportVisaById";

		public static void AddFindPassportVisaByIdEndpoint(this IEndpointRouteBuilder epBuilder)
		{
			epBuilder.MapGet(
				EndpointRoute.PassportVisa.GetById, FindPassportVisaById)
				.RequireAuthorization(EndpointAuthorization.Passport)
				.WithName(Name)
				.WithTags("PassportVisa")
				.Produces(StatusCodes.Status401Unauthorized)
				.Produces<PassportVisaResponse>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		public static async Task<IResult> FindPassportVisaById(
			Guid guId,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			Guid guPassportId = Guid.Empty;

			if (httpContext.TryParsePassportId(out guPassportId) == false)
				return Results.BadRequest("Passport could not be identified.");

			PassportVisaByIdQuery qryPassportVisa = MapToQuery(guId, guPassportId);

			IMessageResult<PassportVisaByIdResult> mdtResult = await mdtMediator.Send(qryPassportVisa, tknCancellation);

			return mdtResult.Match(
				msgError => Results.BadRequest($"{msgError.Code}: {msgError.Description}"),
				ppPassportVisa =>
				{
					PassportVisaResponse rspnPassportVisa = ppPassportVisa.MapToResponse();
					return TypedResults.Ok(rspnPassportVisa);
				});
		}

		private static PassportVisaByIdQuery MapToQuery(Guid guPassportIdToFind, Guid guPassportId)
		{
			return new PassportVisaByIdQuery()
			{
				RestrictedPassportId = guPassportId,
				PassportVisaId = guPassportIdToFind
			};
		}

		private static PassportVisaResponse MapToResponse(this PassportVisaByIdResult rsltVisa)
		{
			return new PassportVisaResponse()
			{
				ConcurrencyStamp = rsltVisa.PassportVisa.ConcurrencyStamp,
				Id = rsltVisa.PassportVisa.Id,
				Level = rsltVisa.PassportVisa.Level,
				Name = rsltVisa.PassportVisa.Name
			};
		}
	}
}
