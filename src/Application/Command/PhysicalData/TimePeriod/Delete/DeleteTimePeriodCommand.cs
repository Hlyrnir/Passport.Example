using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Command.PhysicalData.TimePeriod.Delete
{
	public sealed class DeleteTimePeriodCommand : ICommand<IMessageResult<bool>>, IRestrictedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

		public required Guid TimePeriodId { get; init; }
	}
}
