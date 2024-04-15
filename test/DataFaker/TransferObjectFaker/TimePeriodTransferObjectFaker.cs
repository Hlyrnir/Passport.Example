using Domain.Interface.Transfer;

namespace DomainFaker.TransferObjectFaker
{
	internal sealed class TimePeriodTransferObjectFaker : ITimePeriodTransferObject
	{
		public string ConcurrencyStamp { get; init; } = string.Empty;
		public Guid Id { get; init; }
		public double[] Magnitude { get; init; } = { 0 };
		public double Offset { get; init; }
		public Guid PhysicalDimensionId { get; init; }
	}
}