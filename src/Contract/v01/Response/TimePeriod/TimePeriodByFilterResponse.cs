using Contract.v01.Common;

namespace Contract.v01.Response.TimePeriod
{
	public class TimePeriodByFilterResponse : PagedResponse
	{
		public required IEnumerable<TimePeriodByIdResponse> TimePeriod { get; init; }
	}
}
