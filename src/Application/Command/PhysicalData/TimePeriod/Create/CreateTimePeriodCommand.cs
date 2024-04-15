using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Command.PhysicalData.TimePeriod.Create
{
	public sealed class CreateTimePeriodCommand : ICommand<IMessageResult<Guid>>, IRestrictedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

		public required Guid PhysicalDimensionId { get; init; } = Guid.Empty;
		public required double[] Magnitude { get; init; } = new double[] { 0.0 };
		public required double Offset { get; init; } = 0.0;
	}
}
