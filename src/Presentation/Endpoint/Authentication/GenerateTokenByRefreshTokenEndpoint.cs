using Application.Command.Authentication.BearerTokenByRefreshToken;
using Application.Interface.Result;
using Contract.v01.Request.Authentication;
using Mediator;

namespace Presentation.Endpoint.Authentication
{
	public static class GenerateTokenByRefreshTokenEndpoint
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
				.Produces<string>(StatusCodes.Status200OK)
				.Produces<string>(StatusCodes.Status400BadRequest)
				.WithApiVersionSet(EndpointVersion.VersionSet)
				.HasApiVersion(1.0);
		}

		public static async Task<IResult> GenerateTokenByRefreshToken(
			GenerateBearerTokenByRefreshTokenRequest rqstTokenByRefreshToken,
			HttpContext httpContext,
			ISender mdtMediator,
			CancellationToken tknCancellation)
		{
			BearerTokenByRefreshTokenCommand cmdInsert = rqstTokenByRefreshToken.MapToCommand();

			IMessageResult<string> mdtResult = await mdtMediator.Send(cmdInsert, tknCancellation);

			return mdtResult.Match(
				msgError => Results.BadRequest($"{msgError.Code}: {msgError.Description}"),
				sToken => TypedResults.Ok(sToken));
		}

		private static BearerTokenByRefreshTokenCommand MapToCommand(this GenerateBearerTokenByRefreshTokenRequest cmdRequest)
		{
			return new BearerTokenByRefreshTokenCommand()
			{
				PassportId = cmdRequest.PassportId,
				Provider = cmdRequest.Provider,
				RefreshToken = cmdRequest.RefreshToken
			};
		}
	}
}
