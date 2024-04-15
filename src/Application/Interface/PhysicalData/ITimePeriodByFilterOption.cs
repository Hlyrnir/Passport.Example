namespace Application.Interface.PhysicalData
{
    public interface ITimePeriodByFilterOption : IPagedFilter
    {
        Guid? PhysicalDimensionId { get; init; }
    }
}