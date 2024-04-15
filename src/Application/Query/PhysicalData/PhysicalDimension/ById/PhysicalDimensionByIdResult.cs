using Domain.Interface.PhysicalData;

namespace Application.Query.PhysicalData.PhysicalDimension.ById
{
	public sealed class PhysicalDimensionByIdResult
	{
		public required IPhysicalDimension PhysicalDimension { get; init; }
	}
}
