namespace Contract.v01.Response.PhysicalData
{
	public sealed class PhysicalDimensionByIdResponse
	{
		public required double ConversionFactorToSI { get; init; } = 0.0;
		public required string CultureName { get; init; } = "DEFAULT_CULTURE_NAME";
		public required double ExponentOfSecond { get; init; } = 0.0f;
		public required double ExponentOfMetre { get; init; } = 0.0f;
		public required double ExponentOfKilogram { get; init; } = 0.0f;
		public required double ExponentOfAmpere { get; init; } = 0.0f;
		public required double ExponentOfKelvin { get; init; } = 0.0f;
		public required double ExponentOfMole { get; init; } = 0.0f;
		public required double ExponentOfCandela { get; init; } = 0.0f;
		public required Guid Id { get; init; } = Guid.Empty;
		public required string Name { get; init; } = "DEFAULT_NAME";
		public required string Symbol { get; init; } = "DEFAULT_SYMBOL";
		public required string Unit { get; init; } = "DEFAULT_UNIT";
	}
}
