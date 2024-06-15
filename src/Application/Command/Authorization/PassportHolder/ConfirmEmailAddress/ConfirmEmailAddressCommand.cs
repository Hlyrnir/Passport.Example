using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Command.Authorization.PassportHolder.ConfirmEmailAddress
{
	public sealed class ConfirmEmailAddressCommand : ICommand<IMessageResult<bool>>, IRestrictedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

        public required string ConcurrencyStamp { get; init; }
        public required Guid PassportHolderId { get; init; }
		public required string EmailAddress { get; init; }
	}
}
