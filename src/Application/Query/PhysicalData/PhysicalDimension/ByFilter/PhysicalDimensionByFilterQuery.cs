using Application.Interface.Message;
using Application.Interface.PhysicalData;
using Application.Interface.Result;
using Mediator;

namespace Application.Query.PhysicalData.PhysicalDimension.ByFilter
{
	public sealed class PhysicalDimensionByFilterQuery : IQuery<IMessageResult<PhysicalDimensionByFilterResult>>, IRestrictedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

		public required IPhysicalDimensionByFilterOption Filter { get; init; }
	}
}
