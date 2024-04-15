using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Query.PhysicalData.PhysicalDimension.ById
{
	public sealed class PhysicalDimensionByIdQuery : IQuery<IMessageResult<PhysicalDimensionByIdResult>>, IRestrictedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }
		public required Guid PhysicalDimensionId { get; init; }
	}
}
