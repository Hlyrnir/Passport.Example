using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Command.Authorization.PassportHolder.DeletePassportHolder
{
	public sealed class DeletePassportHolderCommand : ICommand<IMessageResult<bool>>, IRestrictedAuthorization, IVerifiedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

		public required Guid PassportHolderId { get; init; }
	}
}