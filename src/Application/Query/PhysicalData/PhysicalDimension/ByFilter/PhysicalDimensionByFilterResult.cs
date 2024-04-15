using Domain.Interface.PhysicalData;

namespace Application.Query.PhysicalData.PhysicalDimension.ByFilter
{
	public sealed class PhysicalDimensionByFilterResult
	{
		public required IEnumerable<IPhysicalDimension> PhysicalDimension { get; init; }
		public required int MaximalNumberOfPhysicalDimension { get; init; }
	}
}
