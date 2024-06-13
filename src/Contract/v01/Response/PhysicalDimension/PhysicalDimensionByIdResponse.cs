namespace Contract.v01.Response.PhysicalDimension
{
    public sealed class PhysicalDimensionByIdResponse
    {
        public required double ConversionFactorToSI { get; init; } = 0.0;
        public required string CultureName { get; init; } = "DEFAULT_CULTURE_NAME";
        public required float ExponentOfAmpere { get; init; } = 0.0f;
        public required float ExponentOfCandela { get; init; } = 0.0f;
        public required float ExponentOfKelvin { get; init; } = 0.0f;
        public required float ExponentOfKilogram { get; init; } = 0.0f;
        public required float ExponentOfMetre { get; init; } = 0.0f;
        public required float ExponentOfMole { get; init; } = 0.0f;
        public required float ExponentOfSecond { get; init; } = 0.0f;
        public required Guid Id { get; init; } = Guid.Empty;
        public required string Name { get; init; } = "DEFAULT_NAME";
        public required string Symbol { get; init; } = "DEFAULT_SYMBOL";
        public required string Unit { get; init; } = "DEFAULT_UNIT";
    }
}
