using Application.Interface.Message;
using Application.Interface.PhysicalData;
using Application.Interface.Result;
using Mediator;

namespace Application.Query.PhysicalData.TimePeriod.ByFilter
{
	public sealed class TimePeriodByFilterQuery : IQuery<IMessageResult<TimePeriodByFilterResult>>, IRestrictedAuthorization
	{
		public required Guid RestrictedPassportId { get; init; }

		public required ITimePeriodByFilterOption Filter { get; init; }
	}
}
