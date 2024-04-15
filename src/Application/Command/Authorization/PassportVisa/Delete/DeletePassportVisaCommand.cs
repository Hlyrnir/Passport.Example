using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Command.Authorization.PassportVisa.Delete
{
	public sealed class DeletePassportVisaCommand : ICommand<IMessageResult<bool>>, IRestrictedAuthorization, IVerifiedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

		public required Guid PassportVisaId { get; init; }
	}
}
