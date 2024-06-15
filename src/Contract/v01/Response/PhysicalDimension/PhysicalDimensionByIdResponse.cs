namespace Contract.v01.Response.PhysicalDimension
{
    public sealed class PhysicalDimensionByIdResponse
    {
        public required string ConcurrencyStamp { get; init; }
        public required double ConversionFactorToSI { get; init; }
        public required string CultureName { get; init; }
        public required float ExponentOfAmpere { get; init; }
        public required float ExponentOfCandela { get; init; }
        public required float ExponentOfKelvin { get; init; }
        public required float ExponentOfKilogram { get; init; }
        public required float ExponentOfMetre { get; init; }
        public required float ExponentOfMole { get; init; }
        public required float ExponentOfSecond { get; init; }
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Symbol { get; init; }
        public required string Unit { get; init; }
    }
}
