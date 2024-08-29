using Application.Command.Authentication.JwtTokenByCredential;
using Application.Interface.Result;
using Application.Token;
using Contract.v01.Request.Authentication;
using Contract.v01.Response.Authentication;
using Domain.Interface.Authorization;
using Mediator;

namespace Presentation.Endpoint.Authentication
{
	public static class GenerateJwtTokenByCredentialEndpoint
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
				.Produces<JwtTokenResponse>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		public static async Task<IResult> GenerateTokenByCredential(
			GenerateJwtTokenByCredentialRequest rqstTokenByCredential,
			IPassportCredential ppCredential,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			JwtTokenByCredentialCommand cmdToken = rqstTokenByCredential.MapToCommand(ppCredential);

			IMessageResult<JwtTokenTransferObject> mdtResult = await mdtMediator.Send(cmdToken, tknCancellation);

			return mdtResult.Match(
				msgError => Results.BadRequest($"{msgError.Code}: {msgError.Description}"),
				dtoJwtToken => TypedResults.Ok(dtoJwtToken.MapToResponse()));
		}

		private static JwtTokenByCredentialCommand MapToCommand(this GenerateJwtTokenByCredentialRequest cmdRequest, IPassportCredential ppCredential)
		{
			ppCredential.Initialize(
					sProvider: cmdRequest.Provider,
					sCredential: cmdRequest.Credential,
					sSignature: cmdRequest.Signature);

			return new JwtTokenByCredentialCommand()
			{
				Credential = ppCredential
			};
		}

		private static JwtTokenResponse MapToResponse(this JwtTokenTransferObject dtoJwtToken)
		{
			return new JwtTokenResponse()
			{
				ExpiredAt = dtoJwtToken.ExpiredAt,
				Provider = dtoJwtToken.Provider,
				RefreshToken = dtoJwtToken.RefreshToken,
				Token = dtoJwtToken.Token
			};
		}
	}
}
