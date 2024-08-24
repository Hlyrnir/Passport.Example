using Application.Common.Error;
using Application.Interface.Result;
using Application.Query.Authorization.PassportVisa.ByPassport;
using Contract.v01.Response.Authorization;
using Domain.Interface.Authorization;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.Authorization.PassportVisa
{
    public static class FindPassportVisaByPassportIdEndpoint
	{
		public const string Name = "FindPassportVisaByPassportId";

		public static void AddFindPassportVisaByPassportIdEndpoint(this IEndpointRouteBuilder epBuilder)
		{
			epBuilder.MapGet(
				EndpointRoute.PassportVisa.GetByPassportId, FindPassportVisaByPassportId)
				.RequireAuthorization(EndpointAuthorization.Passport)
				.WithName(Name)
				.WithTags("PassportVisa")
				.Produces(StatusCodes.Status401Unauthorized)
				.Produces(StatusCodes.Status403Forbidden)
				.Produces<IEnumerable<PassportVisaResponse>>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		public static async Task<IResult> FindPassportVisaByPassportId(
			Guid guPassportIdToFind,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			Guid guId = Guid.Empty;

			if (httpContext.TryParsePassportId(out guId) == false)
				return Results.BadRequest("Passport could not be identified.");

			PassportVisaByPassportIdQuery qryPassportVisa = MapToQuery(guId, guPassportIdToFind);

			IMessageResult<PassportVisaByPassportIdResult> mdtResult = await mdtMediator.Send(qryPassportVisa, tknCancellation);

			return mdtResult.Match(
				msgError =>
				{
					if (msgError.Equals(AuthorizationError.PassportVisa.VisaDoesNotExist) == true)
						return Results.Forbid();

					return Results.BadRequest($"{msgError.Code}: {msgError.Description}");
				},
				rsltPassportVisa =>
				{
					IEnumerable<PassportVisaResponse> rspnPassportVisa = rsltPassportVisa.MapToResponse();
					return TypedResults.Ok(rspnPassportVisa);
				});
		}

		private static PassportVisaByPassportIdQuery MapToQuery(Guid guPassportId, Guid guPassportIdToFind)
		{
			return new PassportVisaByPassportIdQuery()
			{
				RestrictedPassportId = guPassportId,
				PassportIdToFind = guPassportIdToFind
			};
		}

		private static IEnumerable<PassportVisaResponse> MapToResponse(this PassportVisaByPassportIdResult rsltPassportVisa)
		{
			foreach (IPassportVisa ppVisa in rsltPassportVisa.PassportVisa)
			{
				yield return new PassportVisaResponse()
				{
					ConcurrencyStamp = ppVisa.ConcurrencyStamp,
					Id = ppVisa.Id,
					Level = ppVisa.Level,
					Name = ppVisa.Name
				};
			}
		}
	}
}