using Domain.Interface.Transfer;

namespace Infrastructure.Transfer.PhysicalData
{
	internal sealed class TimePeriodTransferObject : ITimePeriodTransferObject
	{
		public string ConcurrencyStamp { get; init; } = string.Empty;
		public Guid Id { get; init; }
		public double[] Magnitude { get; init; } = { 0 };
		public double Offset { get; init; }
		public Guid PhysicalDimensionId { get; init; }
	}
}
