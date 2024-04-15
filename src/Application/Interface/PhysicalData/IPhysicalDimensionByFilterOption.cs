namespace Application.Interface.PhysicalData
{
    public interface IPhysicalDimensionByFilterOption : IPagedFilter
    {
        double? ConversionFactorToSI { get; init; }
        string? CultureName { get; init; }
        float? ExponentOfAmpere { get; init; }
        float? ExponentOfCandela { get; init; }
        float? ExponentOfKelvin { get; init; }
        float? ExponentOfKilogram { get; init; }
        float? ExponentOfMetre { get; init; }
        float? ExponentOfMole { get; init; }
        float? ExponentOfSecond { get; init; }
        string? Name { get; init; }
        string? Symbol { get; init; }
        string? Unit { get; init; }
    }
}