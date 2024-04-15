using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.PhysicalData;
using Application.Interface.Result;
using Application.Interface.Time;
using Domain.Interface.PhysicalData;
using Mediator;

namespace Application.Command.PhysicalData.TimePeriod.Create
{
	internal sealed class CreateTimePeriodCommandHandler : ICommandHandler<CreateTimePeriodCommand, IMessageResult<Guid>>
	{
		private readonly ITimeProvider prvTime;
		private readonly IPhysicalDimensionRepository repoPhysicalDimension;
		private readonly ITimePeriodRepository repoTimePeriod;

		public CreateTimePeriodCommandHandler(
			ITimeProvider prvTime,
			IPhysicalDimensionRepository repoPhysicalDimension,
			ITimePeriodRepository repoTimePeriod)
		{
			this.prvTime = prvTime;
			this.repoPhysicalDimension = repoPhysicalDimension;
			this.repoTimePeriod = repoTimePeriod;
		}

		public async ValueTask<IMessageResult<Guid>> Handle(CreateTimePeriodCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<Guid>(DefaultMessageError.TaskAborted);

			IRepositoryResult<IPhysicalDimension> rsltPhysicalDimension = await repoPhysicalDimension.FindByIdAsync(msgMessage.PhysicalDimensionId, tknCancellation);

			return await rsltPhysicalDimension.MatchAsync(
				msgError => new MessageResult<Guid>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				async pdPhysicalDimension =>
				{
					ITimePeriod? pdTimePeriod = Domain.Aggregate.PhysicalData.TimePeriod.TimePeriod.Create(
						dMagnitude: msgMessage.Magnitude,
						dOffset: msgMessage.Offset,
						pdPhysicalDimension: pdPhysicalDimension);

					if (pdTimePeriod is null)
						return new MessageResult<Guid>(new MessageError() { Code = DomainError.Code.Method, Description = "Time period could not be created." });

					IRepositoryResult<bool> rsltInsert = await repoTimePeriod.InsertAsync(pdTimePeriod, prvTime.GetUtcNow(), tknCancellation);

					return rsltInsert.Match(
						msgError => new MessageResult<Guid>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
						bResult => new MessageResult<Guid>(pdTimePeriod.Id));
				});
		}
	}
}