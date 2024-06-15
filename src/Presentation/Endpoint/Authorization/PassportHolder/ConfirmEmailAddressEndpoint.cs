using Application.Command.Authorization.PassportHolder.ConfirmEmailAddress;
using Application.Interface.Result;
using Contract.v01.Request.Authorization.PassportHolder;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.Authorization.PassportHolder
{
    public static class ConfirmEmailAddressEndpoint
    {
        public const string Name = "ConfirmEmailAddress";

        public static void AddConfirmEmailAddressEndpoint(this IEndpointRouteBuilder epBuilder)
        {
            epBuilder.MapPut(
                EndpointRoute.PassportHolder.ConfirmEmailAddress, ConfirmEmailAddress)
                .RequireAuthorization(EndpointAuthorization.Passport)
                .WithName(Name)
                .WithTags("PassportHolder")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces<bool>(StatusCodes.Status200OK)
                .Produces<string>(StatusCodes.Status400BadRequest)
                .WithApiVersionSet(EndpointVersion.VersionSet)
                .HasApiVersion(1.0);
        }

        public static async Task<IResult> ConfirmEmailAddress(
            ConfirmEmailAddressRequest rqstConfirmEmailAddress,
            HttpContext httpContext,
            ISender mdtMediator,
            CancellationToken tknCancellation)
        {
            Guid guPassportId = Guid.Empty;

            if (httpContext.TryParsePassportId(out guPassportId) == false)
                return Results.BadRequest("Passport could not be identified.");

            ConfirmEmailAddressCommand cmdUpdate = rqstConfirmEmailAddress.MapToCommand(guPassportId);

            IMessageResult<bool> mdtResult = await mdtMediator.Send(cmdUpdate, tknCancellation);

            return mdtResult.Match(
                msgError => Results.BadRequest($"{msgError.Code}: {msgError.Description}"),
                bResult => TypedResults.Ok(bResult));
        }

        private static ConfirmEmailAddressCommand MapToCommand(this ConfirmEmailAddressRequest rqstConfirmEmailAddress, Guid guPassportId)
        {
            return new ConfirmEmailAddressCommand()
            {
                RestrictedPassportId = guPassportId,
                ConcurrencyStamp = rqstConfirmEmailAddress.ConcurrencyStamp,
                EmailAddress = rqstConfirmEmailAddress.EmailAddress,
                PassportHolderId = rqstConfirmEmailAddress.PassportHolderId
            };
        }
    }
}