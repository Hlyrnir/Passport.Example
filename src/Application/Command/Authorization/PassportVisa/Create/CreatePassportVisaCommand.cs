using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Command.Authorization.PassportVisa.Create
{
	public sealed class CreatePassportVisaCommand : ICommand<IMessageResult<Guid>>, IRestrictedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

		public required string Name { get; init; }
		public required int Level { get; init; }
	}
}
