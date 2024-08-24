using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Common.Result.Repository;
using Application.Interface.PhysicalData;
using Application.Interface.Result;
using Domain.Interface.PhysicalData;
using Mediator;

namespace Application.Query.PhysicalData.TimePeriod.ById
{
    internal sealed class TimePeriodByIdQueryHandler : IQueryHandler<TimePeriodByIdQuery, IMessageResult<TimePeriodByIdResult>>
	{
		private readonly ITimePeriodRepository repoTimePeriod;

		public TimePeriodByIdQueryHandler(ITimePeriodRepository repoTimePeriod)
		{
			this.repoTimePeriod = repoTimePeriod;
		}

		public async ValueTask<IMessageResult<TimePeriodByIdResult>> Handle(TimePeriodByIdQuery msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<TimePeriodByIdResult>(DefaultMessageError.TaskAborted);

			RepositoryResult<ITimePeriod> rsltTimePeriod = await repoTimePeriod.FindByIdAsync(msgMessage.TimePeriodId, tknCancellation);

			return rsltTimePeriod.Match(
				msgError => new MessageResult<TimePeriodByIdResult>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				pdTimePeriod =>
				{
					TimePeriodByIdResult qryResult = new TimePeriodByIdResult()
					{
						TimePeriod = pdTimePeriod
					};

					return new MessageResult<TimePeriodByIdResult>(qryResult);
				});
		}
	}
}