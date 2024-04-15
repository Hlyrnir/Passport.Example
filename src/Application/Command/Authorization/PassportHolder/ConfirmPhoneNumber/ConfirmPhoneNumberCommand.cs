using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Command.Authorization.PassportHolder.ConfirmPhoneNumber
{
	public sealed class ConfirmPhoneNumberCommand : ICommand<IMessageResult<bool>>, IRestrictedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

		public required Guid PassportHolderId { get; init; }
		public required string PhoneNumber { get; init; }
	}
}
