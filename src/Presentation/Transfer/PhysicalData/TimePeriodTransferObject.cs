namespace Presentation.Transfer.PhysicalData
{
	public struct TimePeriodTransferObject
	{
		public TimePeriodTransferObject()
		{

		}

		public PhysicalDimensionTransferObject Dimension { get; init; } = new PhysicalDimensionTransferObject();
		public string PhysicalDimensionId { get; init; } = "DEFAULT_ID";
		public string Id { get; init; } = "DEFAULT_ID";
		public double[] Magnitude { get; init; } = new double[] { 0.0 };
		public double Offset { get; init; } = 0.0;
	}
}
