using Application.Interface.Message;
using Application.Interface.Result;
using Domain.Interface.Authorization;
using Mediator;

namespace Application.Command.Authorization.PassportToken.ResetCredential
{
	public sealed class ResetCredentialCommand : ICommand<IMessageResult<bool>>, IRestrictedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

		public required IPassportCredential CredentialToVerify { get; init; }
		public required IPassportCredential CredentialToApply { get; init; }
	}
}