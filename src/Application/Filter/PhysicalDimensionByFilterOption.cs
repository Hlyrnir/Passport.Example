using Application.Interface.PhysicalData;

namespace Application.Filter
{
    public class PhysicalDimensionByFilterOption : IPhysicalDimensionByFilterOption
	{
		public required double? ConversionFactorToSI { get; init; }
		public required string? CultureName { get; init; }
		public required float? ExponentOfAmpere { get; init; }
		public required float? ExponentOfCandela { get; init; }
		public required float? ExponentOfKelvin { get; init; }
		public required float? ExponentOfKilogram { get; init; }
		public required float? ExponentOfMetre { get; init; }
		public required float? ExponentOfMole { get; init; }
		public required float? ExponentOfSecond { get; init; }
		public required string? Name { get; init; }
		public required string? Symbol { get; init; }
		public required string? Unit { get; init; }

		public int Page { get; init; } = 1;
		public int PageSize { get; init; } = 10;
	}
}
