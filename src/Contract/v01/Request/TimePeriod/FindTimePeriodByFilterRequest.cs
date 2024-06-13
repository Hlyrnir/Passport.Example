using Contract.v01.Common;

namespace Contract.v01.Request.TimePeriod
{
    public sealed class FindTimePeriodByFilterRequest : PagedRequest
    {
        public required Guid? PhysicalDimensionId { get; init; }
    }
}
