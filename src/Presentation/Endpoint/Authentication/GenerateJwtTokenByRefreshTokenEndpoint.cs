using Application.Command.Authentication.JwtTokenByRefreshToken;
using Application.Interface.Result;
using Application.Token;
using Contract.v01.Request.Authentication;
using Contract.v01.Response.Authentication;
using Mediator;

namespace Presentation.Endpoint.Authentication
{
	public static class GenerateJwtTokenByRefreshTokenEndpoint
	{
		public const string Name = "GenerateTokenByRefreshToken";

		public static void AddGenerateTokenByRefreshTokenEndpoint(this IEndpointRouteBuilder epBuilder)
		{
			epBuilder.MapPost(
				EndpointRoute.Authentication.RefreshToken, GenerateTokenByRefreshToken)
				.AllowAnonymous()
				.WithName(Name)
				.WithTags("Authentication")
				.Produces(StatusCodes.Status401Unauthorized)
				.Produces<JwtTokenResponse>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		public static async Task<IResult> GenerateTokenByRefreshToken(
			GenerateJwtTokenByRefreshTokenRequest rqstTokenByRefreshToken,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			JwtTokenByRefreshTokenCommand cmdInsert = rqstTokenByRefreshToken.MapToCommand();

			IMessageResult<JwtTokenTransferObject> mdtResult = await mdtMediator.Send(cmdInsert, tknCancellation);

			return mdtResult.Match(
				msgError => Results.BadRequest($"{msgError.Code}: {msgError.Description}"),
				dtoJwtToken => TypedResults.Ok(dtoJwtToken.MapToResponse()));
		}

		private static JwtTokenByRefreshTokenCommand MapToCommand(this GenerateJwtTokenByRefreshTokenRequest cmdRequest)
		{
			return new JwtTokenByRefreshTokenCommand()
			{
				PassportId = cmdRequest.PassportId,
				Provider = cmdRequest.Provider,
				RefreshToken = cmdRequest.RefreshToken
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
