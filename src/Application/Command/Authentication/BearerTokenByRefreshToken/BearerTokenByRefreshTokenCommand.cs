using Application.Interface.Result;
using Mediator;

namespace Application.Command.Authentication.BearerTokenByRefreshToken
{
	public sealed class BearerTokenByRefreshTokenCommand : ICommand<IMessageResult<string>>
	{
		public required Guid PassportId { get; init; }
		public required string Provider { get; init; }
		public required string RefreshToken { get; init; }
	}
}
