using Domain.Interface.PhysicalData;

namespace Application.Query.PhysicalData.TimePeriod.ById
{
	public sealed class TimePeriodByIdResult
	{
		public required ITimePeriod TimePeriod { get; init; }
	}
}
