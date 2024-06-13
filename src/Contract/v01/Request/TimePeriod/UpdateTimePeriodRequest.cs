namespace Contract.v01.Request.TimePeriod
{
    public sealed class UpdateTimePeriodRequest
    {
        public required Guid TimePeriodId { get; init; }
        public required Guid PhysicalDimensionId { get; init; }
        public required double[] Magnitude { get; init; }
        public required double Offset { get; init; }
    }
}
