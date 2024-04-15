using Application.Command.Authorization.Passport.RegisterPassport;
using Application.Interface.Result;
using Contract.v01.Request.Authorization.Passport;
using Domain.Interface.Authorization;
using Mediator;
using Presentation.Common;

namespace Presentation.Endpoint.Authorization.Passport
{
    public static class RegisterPassportEndpoint
	{
		public const string Name = "RegisterPassport";

		public static void AddRegisterPassportEndpoint(this IEndpointRouteBuilder epBuilder)
		{
			epBuilder.MapPost(
				EndpointRoute.Passport.Register, RegisterPassport)
				.AllowAnonymous()
				.RequireAuthorization(EndpointAuthorization.Passport)
				.WithName(Name)
				.WithTags("Passport")
				.Produces(StatusCodes.Status401Unauthorized)
				.Produces(StatusCodes.Status201Created)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		public static async Task<IResult> RegisterPassport(
			RegisterPassportRequest rqstPassport,
			IPassportCredential ppCredentialToRegister,
			IPassportCredential ppCredentialToVerify,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			Guid guPassportId = Guid.Empty;

			if (httpContext.TryParsePassportId(out guPassportId) == false)
				return Results.BadRequest("Passport could not be identified.");

			RegisterPassportCommand cmdRegister = rqstPassport.MapToCommand(
				guPassportId: guPassportId,
				ppCredentialToRegister: ppCredentialToRegister,
				ppCredentialToVerify: ppCredentialToVerify);

			IMessageResult<Guid> mdtResult = await mdtMediator.Send(cmdRegister, tknCancellation);

			return mdtResult.Match(
				msgError => Results.BadRequest($"{msgError.Code}: {msgError.Description}"),
				guPassportId => TypedResults.CreatedAtRoute(FindPassportByIdEndpoint.Name, new { guId = guPassportId }));
		}

		private static RegisterPassportCommand MapToCommand(this RegisterPassportRequest cmdRequest, Guid guPassportId, IPassportCredential ppCredentialToRegister, IPassportCredential ppCredentialToVerify)
		{
			ppCredentialToRegister.Initialize(
				sProvider: cmdRequest.Provider,
				sCredential: cmdRequest.CredentialToRegister,
				sSignature: cmdRequest.SignatureToRegister);

			ppCredentialToVerify.Initialize(
				sProvider: cmdRequest.Provider,
				sCredential: cmdRequest.CredentialToVerify,
				sSignature: cmdRequest.SignatureToVerify);

			return new RegisterPassportCommand()
			{
				RestrictedPassportId = guPassportId,
				IssuedBy = Guid.NewGuid(),
				CredentialToRegister = ppCredentialToRegister,
				//CredentialToVerify = ppCredentialToVerify,
				CultureName = cmdRequest.CultureName,
				EmailAddress = cmdRequest.EmailAddress,
				FirstName = cmdRequest.FirstName,
				LastName = cmdRequest.LastName,
				PhoneNumber = cmdRequest.PhoneNumber
			};
		}

		//private static RegisterPassportResponse MapToResponse(this RegisterPassportCommand cmdRegister)
		//{
		//	return new RegisterPassportResponse()
		//	{
		//		PassportId = Guid.Empty,
		//		PassportHolderId = Guid.Empty
		//	};
		//}
	}
}