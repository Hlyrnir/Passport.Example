using Application.Interface.PhysicalData;

namespace Application.Filter
{
	public class TimePeriodByFilterOption : ITimePeriodByFilterOption
	{
		public required Guid? PhysicalDimensionId { get; init; }

		public int Page { get; init; } = 1;
		public int PageSize { get; init; } = 10;
	}
}
