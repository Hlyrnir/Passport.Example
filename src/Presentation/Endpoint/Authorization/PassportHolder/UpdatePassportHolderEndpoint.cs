using Application.Command.Authorization.PassportHolder.Update;
using Application.Common.Error;
using Application.Interface.Result;
using Contract.v01.Request.Authorization.PassportHolder;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.Authorization.PassportHolder
{
    public static class UpdatePassportHolderEndpoint
    {
        public const string Name = "UpdatePassportHolder";

        public static void AddUpdatePassportHolderEndpoint(this IEndpointRouteBuilder epBuilder)
        {
            epBuilder.MapPut(
                EndpointRoute.PassportHolder.Update, UpdatePassportHolder)
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

        public static async Task<IResult> UpdatePassportHolder(
            UpdatePassportHolderRequest rqstPassportHolder,
            HttpContext httpContext,
            ISender mdtMediator,
            CancellationToken tknCancellation)
        {
            Guid guPassportId = Guid.Empty;

            if (httpContext.TryParsePassportId(out guPassportId) == false)
                return Results.BadRequest("Passport could not be identified.");

            UpdatePassportHolderCommand cmdUpdate = rqstPassportHolder.MapToCommand(guPassportId);

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

        private static UpdatePassportHolderCommand MapToCommand(this UpdatePassportHolderRequest rqstPassportHolder, Guid guPassportId)
        {
            return new UpdatePassportHolderCommand()
            {
                RestrictedPassportId = guPassportId,
                PassportHolderId = rqstPassportHolder.PassportHolderId,
                ConcurrencyStamp = rqstPassportHolder.ConcurrencyStamp,
                CultureName = rqstPassportHolder.CultureName,
                EmailAddress = rqstPassportHolder.EmailAddress,
                FirstName = rqstPassportHolder.FirstName,
                LastName = rqstPassportHolder.LastName,
                PhoneNumber = rqstPassportHolder.PhoneNumber
            };
        }
    }
}