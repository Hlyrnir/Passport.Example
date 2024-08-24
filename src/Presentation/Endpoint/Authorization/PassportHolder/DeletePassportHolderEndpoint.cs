using Application.Command.Authorization.PassportHolder.Delete;
using Application.Common.Error;
using Application.Interface.Result;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.Authorization.PassportHolder
{
    public static class DeletePassportHolderEndpoint
    {
        public const string Name = "DeletePassportHolder";

        public static void AddDeletePassportHolderEndpoint(this IEndpointRouteBuilder epBuilder)
        {
            epBuilder.MapDelete(
                EndpointRoute.PassportHolder.Delete, DeletePassportHolder)
                .RequireAuthorization(EndpointAuthorization.Passport)
                .WithName(Name)
                .WithTags("PassportHolder")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces<bool>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status400BadRequest)
                .WithApiVersionSet(EndpointVersion.VersionSet)
                .HasApiVersion(1.0);
        }

        public static async Task<IResult> DeletePassportHolder(
            Guid guPassportHolderId,
            HttpContext httpContext,
            ISender mdtMediator,
            CancellationToken tknCancellation)
        {
            Guid guPassportId = Guid.Empty;

            if (httpContext.TryParsePassportId(out guPassportId) == false)
                return Results.BadRequest("Passport could not be identified.");

            DeletePassportHolderCommand cmdUpdate = MapToCommand(guPassportId, guPassportHolderId);

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

        private static DeletePassportHolderCommand MapToCommand(Guid guPassportId, Guid guPassportHolderId)
        {
            return new DeletePassportHolderCommand()
            {
                RestrictedPassportId = guPassportId,
                PassportHolderId = guPassportHolderId
            };
        }
    }
}
