namespace Contract.v01.Request.PhysicalData
{
    public sealed class CreateTimePeriodRequest
    {
        public required Guid PhysicalDimensionId { get; init; }
        public required double[] Magnitude { get; init; }
        public required double Offset { get; init; }
    }
}
