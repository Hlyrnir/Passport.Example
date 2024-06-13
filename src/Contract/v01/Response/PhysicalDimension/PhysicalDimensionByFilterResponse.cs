using Contract.v01.Common;

namespace Contract.v01.Response.PhysicalDimension
{
    public sealed class PhysicalDimensionByFilterResponse : PagedResponse
    {
        public required IEnumerable<PhysicalDimensionByIdResponse> PhysicalDimension { get; init; }
    }
}
