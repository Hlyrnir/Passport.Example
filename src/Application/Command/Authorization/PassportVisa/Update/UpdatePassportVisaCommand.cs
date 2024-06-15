using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Command.Authorization.PassportVisa.Update
{
	public sealed class UpdatePassportVisaCommand : ICommand<IMessageResult<bool>>, IRestrictedAuthorization, IVerifiedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

        public required string ConcurrencyStamp { get; init; }
        public required Guid PassportVisaId { get; init; }
		public required string Name { get; init; }
		public required int Level { get; init; }
	}
}