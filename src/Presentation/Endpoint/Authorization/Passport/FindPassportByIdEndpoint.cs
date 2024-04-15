using Application.Interface.Result;
using Application.Query.Authorization.Passport.ById;
using Contract.v01.Response.Passport;
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
				.Produces<PassportResponse>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		public static async Task<IResult> FindPassportById(
			Guid guId,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			Guid guPassportId = Guid.Empty;

			if (httpContext.TryParsePassportId(out guPassportId) == false)
				return Results.BadRequest("Passport could not be identified.");

			PassportByIdQuery qryPassport = MapToQuery(guId, guPassportId);

			IMessageResult<PassportByIdResult> mdtResult = await mdtMediator.Send(qryPassport, tknCancellation);

			return mdtResult.Match(
				msgError => Results.BadRequest($"{msgError.Code}: {msgError.Description}"),
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

		private static PassportResponse MapToResponse(this PassportByIdResult rsltById)
		{
			return new PassportResponse()
			{
				ConcurrencyStamp = rsltById.Passport.ConcurrencyStamp,
				ExpiredAt = rsltById.Passport.ExpiredAt,
				HolderId = rsltById.Passport.HolderId,
				Id = rsltById.Passport.Id,
				IsAuthority = rsltById.Passport.IsAuthority,
				IsEnabled = rsltById.Passport.IsEnabled,
				IssuedBy = rsltById.Passport.IssuedBy,
				LastCheckedAt = rsltById.Passport.LastCheckedAt,
				LastCheckedBy = rsltById.Passport.LastCheckedBy,
				VisaId = rsltById.Passport.VisaId
			};
		}
	}
}