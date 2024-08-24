using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Interface.PhysicalData;
using Application.Interface.Result;
using Domain.Interface.PhysicalData;
using Mediator;

namespace Application.Command.PhysicalData.TimePeriod.Delete
{
    internal sealed class DeleteTimePeriodCommandHandler : ICommandHandler<DeleteTimePeriodCommand, IMessageResult<bool>>
	{
		private readonly ITimePeriodRepository repoTimePeriod;

		public DeleteTimePeriodCommandHandler(ITimePeriodRepository repoTimePeriod)
		{
			this.repoTimePeriod = repoTimePeriod;
		}

		public async ValueTask<IMessageResult<bool>> Handle(DeleteTimePeriodCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			IRepositoryResult<ITimePeriod> rsltTimePeriod = await repoTimePeriod.FindByIdAsync(msgMessage.TimePeriodId, tknCancellation);

			return await rsltTimePeriod.MatchAsync(
				msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				async pdTimePeriod =>
				{
					IRepositoryResult<bool> rsltDelete = await repoTimePeriod.DeleteAsync(pdTimePeriod, tknCancellation);

					return rsltDelete.Match(
						msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
						bResult => new MessageResult<bool>(bResult));
				});
		}
	}
}