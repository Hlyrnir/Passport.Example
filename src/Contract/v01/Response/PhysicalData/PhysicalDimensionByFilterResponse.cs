using Contract.v01.Common;

namespace Contract.v01.Response.PhysicalData
{
	public sealed class PhysicalDimensionByFilterResponse : PagedResponse
	{
		public required IEnumerable<PhysicalDimensionByIdResponse> PhysicalDimension { get; init; }
	}
}
