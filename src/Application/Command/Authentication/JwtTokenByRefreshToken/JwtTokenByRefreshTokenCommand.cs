using Application.Interface.Result;
using Application.Token;
using Mediator;

namespace Application.Command.Authentication.JwtTokenByRefreshToken
{
	public sealed class JwtTokenByRefreshTokenCommand : ICommand<IMessageResult<JwtTokenTransferObject>>
	{
		public required Guid PassportId { get; init; }
		public required string Provider { get; init; }
		public required string RefreshToken { get; init; }
	}
}
