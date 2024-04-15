namespace Domain.Interface.Transfer
{
	public interface ITimePeriodTransferObject
	{
		string ConcurrencyStamp { get; init; }
		Guid Id { get; init; }
		double[] Magnitude { get; init; }
		double Offset { get; init; }
		Guid PhysicalDimensionId { get; init; }
	}
}
