using Application.Interface.Result;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authentication.BearerTokenByCredential
{
	public sealed class BearerTokenByCredentialCommand : ICommand<IMessageResult<string>>
	{
		public required IPassportCredential Credential { get; init; }
	}
}
