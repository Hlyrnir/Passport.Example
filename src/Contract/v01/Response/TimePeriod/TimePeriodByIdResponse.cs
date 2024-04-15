namespace Contract.v01.Response.TimePeriod
{
	public class TimePeriodByIdResponse
	{
		public required Guid Id { get; init; } = Guid.Empty;
		public required double[] Magnitude { get; init; }
		public required double Offset { get; init; }
		public required Guid PhysicalDimensionId { get; init; }
	}
}
