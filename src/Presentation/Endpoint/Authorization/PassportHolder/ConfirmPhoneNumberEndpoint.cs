using Application.Command.Authorization.PassportHolder.ConfirmPhoneNumber;
using Application.Common.Error;
using Application.Interface.Result;
using Contract.v01.Request.Authorization.PassportHolder;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.Authorization.PassportHolder
{
    public static class ConfirmPhoneNumberEndpoint
    {
        public const string Name = "ConfirmPhoneNumber";

        public static void AddConfirmPhoneNumberEndpoint(this IEndpointRouteBuilder epBuilder)
        {
            epBuilder.MapPut(
                EndpointRoute.PassportHolder.ConfirmPhoneNumber, ConfirmPhoneNumber)
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

        public static async Task<IResult> ConfirmPhoneNumber(
            ConfirmPhoneNumberRequest rqstConfirmPhoneNumber,
            HttpContext httpContext,
            ISender mdtMediator,
            CancellationToken tknCancellation)
        {
            Guid guPassportId = Guid.Empty;

            if (httpContext.TryParsePassportId(out guPassportId) == false)
                return Results.BadRequest("Passport could not be identified.");

            ConfirmPhoneNumberCommand cmdUpdate = rqstConfirmPhoneNumber.MapToCommand(guPassportId);

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

        private static ConfirmPhoneNumberCommand MapToCommand(this ConfirmPhoneNumberRequest rqstConfirmPhoneNumber, Guid guPassportId)
        {
            return new ConfirmPhoneNumberCommand()
            {
                RestrictedPassportId = guPassportId,
                ConcurrencyStamp = rqstConfirmPhoneNumber.ConcurrencyStamp,
                PassportHolderId = rqstConfirmPhoneNumber.PassportHolderId,
                PhoneNumber = rqstConfirmPhoneNumber.PhoneNumber
            };
        }
    }
}