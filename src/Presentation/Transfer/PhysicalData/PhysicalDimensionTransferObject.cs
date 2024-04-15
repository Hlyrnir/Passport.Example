namespace Presentation.Transfer.PhysicalData
{
	public struct PhysicalDimensionTransferObject
	{
		public PhysicalDimensionTransferObject()
		{

		}

		public double ConversionFactorToSI { get; init; } = 0.0;
		public string CultureName { get; init; } = "DEFAULT_CULTURE_NAME";
		public string Id { get; init; } = "DEFAULT_ID";
		public string Name { get; init; } = "DEFAULT_NAME";
		public string Symbol { get; init; } = "DEFAULT_SYMBOL";
		public string Unit { get; init; } = "DEFAULT_UNIT";
		public double ExponentOfSecond { get; init; } = 0.0f;
		public double ExponentOfMetre { get; init; } = 0.0f;
		public double ExponentOfKilogram { get; init; } = 0.0f;
		public double ExponentOfAmpere { get; init; } = 0.0f;
		public double ExponentOfKelvin { get; init; } = 0.0f;
		public double ExponentOfMole { get; init; } = 0.0f;
		public double ExponentOfCandela { get; init; } = 0.0f;
	}
}
