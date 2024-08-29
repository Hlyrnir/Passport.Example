using Application.Interface.Result;
using Application.Token;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authentication.JwtTokenByCredential
{
	public sealed class JwtTokenByCredentialCommand : ICommand<IMessageResult<JwtTokenTransferObject>>
	{
		public required IPassportCredential Credential { get; init; }
	}
}
