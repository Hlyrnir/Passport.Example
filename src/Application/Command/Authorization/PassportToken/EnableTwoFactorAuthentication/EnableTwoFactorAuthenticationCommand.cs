using Application.Interface.Message;
using Application.Interface.Result;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authorization.PassportToken.EnableTwoFactorAuthentication
{
	public sealed class EnableTwoFactorAuthenticationCommand : ICommand<IMessageResult<bool>>, IRestrictedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

		public required IPassportCredential CredentialToVerify { get; init; }
		public required bool TwoFactorIsEnabled { get; init; }
	}
}