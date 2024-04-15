using Domain.Interface.PhysicalData;

namespace Application.Query.PhysicalData.TimePeriod.ByFilter
{
	public sealed class TimePeriodByFilterResult
	{
		public required IEnumerable<ITimePeriod> TimePeriod { get; init; }
		public required int MaximalNumberOfTimePeriod { get; init; }
	}
}
