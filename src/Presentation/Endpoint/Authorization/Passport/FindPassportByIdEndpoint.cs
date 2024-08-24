using Application.Common.Error;
using Application.Interface.Result;
using Application.Query.Authorization.Passport.ById;
using Contract.v01.Response.Authorization;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.Authorization.Passport
{
    public static class FindPassportByIdEndpoint
	{
		public const string Name = "FindPassportById";

		public static void AddFindPassportByIdEndpoint(this IEndpointRouteBuilder epBuilder)
		{
			epBuilder.MapGet(
				EndpointRoute.Passport.GetById, FindPassportById)
				.RequireAuthorization(EndpointAuthorization.Passport)
				.WithName(Name)
				.WithTags("Passport")
				.Produces(StatusCodes.Status401Unauthorized)
				.Produces(StatusCodes.Status403Forbidden)
				.Produces<PassportResponse>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		public static async Task<IResult> FindPassportById(
			Guid guPassportIdToFind,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			Guid guPassportId = Guid.Empty;

			if (httpContext.TryParsePassportId(out guPassportId) == false)
				return Results.BadRequest("Passport could not be identified.");

			PassportByIdQuery qryPassport = MapToQuery(guPassportIdToFind, guPassportId);

			IMessageResult<PassportByIdResult> mdtResult = await mdtMediator.Send(qryPassport, tknCancellation);

			return mdtResult.Match(
				msgError =>
				{
					if (msgError.Equals(AuthorizationError.PassportVisa.VisaDoesNotExist) == true)
						return Results.Forbid();

					return Results.BadRequest($"{msgError.Code}: {msgError.Description}");
				},
				ppPassport =>
				{
					PassportResponse rspnPassport = ppPassport.MapToResponse();
					return TypedResults.Ok(rspnPassport);
				});
		}

		private static PassportByIdQuery MapToQuery(Guid guPassportIdToFind, Guid guPassportId)
		{
			return new PassportByIdQuery()
			{
				RestrictedPassportId = guPassportId,
				PassportId = guPassportIdToFind
			};
		}

		private static PassportResponse MapToResponse(this PassportByIdResult rsltPassportById)
		{
			return new PassportResponse()
			{
				ConcurrencyStamp = rsltPassportById.Passport.ConcurrencyStamp,
				ExpiredAt = rsltPassportById.Passport.ExpiredAt,
				HolderId = rsltPassportById.Passport.HolderId,
				Id = rsltPassportById.Passport.Id,
				IsAuthority = rsltPassportById.Passport.IsAuthority,
				IsEnabled = rsltPassportById.Passport.IsEnabled,
				IssuedBy = rsltPassportById.Passport.IssuedBy,
				LastCheckedAt = rsltPassportById.Passport.LastCheckedAt,
				LastCheckedBy = rsltPassportById.Passport.LastCheckedBy,
				VisaId = rsltPassportById.Passport.VisaId
			};
		}
	}
}