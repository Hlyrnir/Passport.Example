using Application.Command.Authentication.BearerTokenByCredential;
using Application.Interface.Result;
using Contract.v01.Request.Authentication;
using Domain.Interface.Authorization;
using Mediator;

namespace Presentation.Endpoint.Authentication
{
	public static class GenerateTokenByCredentialEndpoint
	{
		public const string Name = "GenerateTokenByCredential";

		public static void AddGenerateTokenByCredentialEndpoint(this IEndpointRouteBuilder epBuilder)
		{
			epBuilder.MapPost(
				EndpointRoute.Authentication.Token, GenerateTokenByCredential)
				.AllowAnonymous()
				.WithName(Name)
				.WithTags("Authentication")
				.Produces(StatusCodes.Status401Unauthorized)
				.Produces<string>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		public static async Task<IResult> GenerateTokenByCredential(
			GenerateBearerTokenByCredentialRequest rqstTokenByCredential,
			IPassportCredential ppCredential,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			BearerTokenByCredentialCommand cmdToken = rqstTokenByCredential.MapToCommand(ppCredential);

			IMessageResult<string> mdtResult = await mdtMediator.Send(cmdToken, tknCancellation);

			return mdtResult.Match(
				msgError => Results.BadRequest($"{msgError.Code}: {msgError.Description}"),
				sToken => TypedResults.Ok(sToken));
		}

		private static BearerTokenByCredentialCommand MapToCommand(this GenerateBearerTokenByCredentialRequest cmdRequest, IPassportCredential ppCredential)
		{
			ppCredential.Initialize(
					sProvider: cmdRequest.Provider,
					sCredential: cmdRequest.Credential,
					sSignature: cmdRequest.Signature);

			return new BearerTokenByCredentialCommand()
			{
				Credential = ppCredential
			};
		}
	}
}
