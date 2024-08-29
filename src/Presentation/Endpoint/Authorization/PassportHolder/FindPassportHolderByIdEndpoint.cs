using Application.Common.Error;
using Application.Interface.Result;
using Application.Query.Authorization.PassportHolder.ById;
using Contract.v01.Response.Authorization;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.Authorization.PassportHolder
{
    public static class FindPassportHolderByIdEndpoint
    {
        public const string Name = "FindPassportHolderById";

        public static void AddFindPassportHolderByIdEndpoint(this IEndpointRouteBuilder epBuilder)
        {
            epBuilder.MapGet(
                EndpointRoute.PassportHolder.GetById, FindPassportHolderById)
                .RequireAuthorization(EndpointAuthorization.Passport)
                .WithName(Name)
                .WithTags("PassportHolder")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces<PassportHolderResponse>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status400BadRequest)
                .WithApiVersionSet(EndpointVersion.VersionSet)
                .HasApiVersion(1.0);
        }

        public static async Task<IResult> FindPassportHolderById(
            Guid guPassportHolderIdToFind,
            HttpContext httpContext,
            ISender mdtMediator,
            CancellationToken tknCancellation)
        {
            Guid guPassportId = Guid.Empty;

            if (httpContext.TryParsePassportId(out guPassportId) == false)
                return Results.BadRequest("Passport could not be identified.");

            PassportHolderByIdQuery qryPassportHolder = MapToQuery(guPassportHolderIdToFind, guPassportId);

            IMessageResult<PassportHolderByIdResult> mdtResult = await mdtMediator.Send(qryPassportHolder, tknCancellation);

            return mdtResult.Match(
				msgError =>
                {
					if (msgError.Equals(AuthorizationError.PassportVisa.VisaDoesNotExist) == true)
						return Results.Forbid();

					return Results.BadRequest($"{msgError.Code}: {msgError.Description}");
                },
                ppPassportHolder =>
                {
                    PassportHolderResponse rspnPassportHolder = ppPassportHolder.MapToResponse();
                    return TypedResults.Ok(rspnPassportHolder);
                });
        }

        private static PassportHolderByIdQuery MapToQuery(Guid guPassportHolderIdToFind, Guid guPassportId)
        {
            return new PassportHolderByIdQuery()
            {
                RestrictedPassportId = guPassportId,
                PassportHolderId = guPassportHolderIdToFind
            };
        }

        private static PassportHolderResponse MapToResponse(this PassportHolderByIdResult rsltPassportHolder)
        {
            return new PassportHolderResponse()
            {
                PassportHolderId = rsltPassportHolder.PassportHolder.Id,
                ConcurrencyStamp = rsltPassportHolder.PassportHolder.ConcurrencyStamp,
                CultureName = rsltPassportHolder.PassportHolder.CultureName,
                EmailAddress = rsltPassportHolder.PassportHolder.EmailAddress,
                FirstName = rsltPassportHolder.PassportHolder.FirstName,
                LastName = rsltPassportHolder.PassportHolder.LastName,
                PhoneNumber = rsltPassportHolder.PassportHolder.PhoneNumber
            };
        }
    }
}
