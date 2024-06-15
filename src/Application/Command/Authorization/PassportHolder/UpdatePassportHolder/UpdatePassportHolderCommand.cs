using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Command.Authorization.PassportHolder.UpdatePassportHolder
{
	public sealed class UpdatePassportHolderCommand : ICommand<IMessageResult<bool>>, IRestrictedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

        public required string ConcurrencyStamp { get; init; }
        public required Guid PassportHolderId { get; init; }
		public required string CultureName { get; init; }
		public required string EmailAddress { get; init; }
		public required string FirstName { get; init; }
		public required string LastName { get; init; }
		public required string PhoneNumber { get; init; }
	}
}
