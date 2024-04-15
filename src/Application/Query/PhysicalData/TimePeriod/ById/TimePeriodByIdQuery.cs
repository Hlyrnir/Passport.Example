using Application.Interface.Message;
using Application.Interface.Result;
using Mediator;

namespace Application.Query.PhysicalData.TimePeriod.ById
{
	public sealed class TimePeriodByIdQuery : IQuery<IMessageResult<TimePeriodByIdResult>>, IRestrictedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

		public required Guid TimePeriodId { get; init; }
	}
}
