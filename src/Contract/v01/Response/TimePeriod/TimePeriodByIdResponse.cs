namespace Contract.v01.Response.TimePeriod
{
	public class TimePeriodByIdResponse
	{
        public required string ConcurrencyStamp { get; init; }
        public required Guid Id { get; init; }
		public required double[] Magnitude { get; init; }
		public required double Offset { get; init; }
		public required Guid PhysicalDimensionId { get; init; }
	}
}
