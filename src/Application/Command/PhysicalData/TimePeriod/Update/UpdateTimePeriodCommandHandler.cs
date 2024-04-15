using Application.Common.Result.Message;
using Application.Common.Result.Repository;
using Application.Error;
using Application.Interface.PhysicalData;
using Application.Interface.Result;
using Application.Interface.Time;
using Domain.Interface.PhysicalData;
using Mediator;

namespace Application.Command.PhysicalData.TimePeriod.Update
{
	internal sealed class UpdateTimePeriodCommandHandler : ICommandHandler<UpdateTimePeriodCommand, IMessageResult<bool>>
	{
		private readonly ITimeProvider prvTime;
		private readonly IPhysicalDimensionRepository repoPhysicalDimension;
		private readonly ITimePeriodRepository repoTimePeriod;

		public UpdateTimePeriodCommandHandler(
			ITimeProvider prvTime,
			IPhysicalDimensionRepository repoPhysicalDimension,
			ITimePeriodRepository repoTimePeriod)
		{
			this.prvTime = prvTime;
			this.repoPhysicalDimension = repoPhysicalDimension;
			this.repoTimePeriod = repoTimePeriod;
		}

		public async ValueTask<IMessageResult<bool>> Handle(UpdateTimePeriodCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			RepositoryResult<IPhysicalDimension> rsltPhysicalDimension = await repoPhysicalDimension.FindByIdAsync(msgMessage.PhysicalDimensionId, tknCancellation);

			return await rsltPhysicalDimension.MatchAsync(
				msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				async pdPhysicalDimension =>
				{
					RepositoryResult<ITimePeriod> rsltTimePeriod = await repoTimePeriod.FindByIdAsync(msgMessage.TimePeriodId, tknCancellation);

					return await rsltTimePeriod.MatchAsync(
						msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
						async pdTimePeriod =>
						{
							if (pdTimePeriod.TryChangePhysicalDimension(pdPhysicalDimension) == false)
								return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = "Physical dimension could not be changed." });

							pdTimePeriod.Magnitude = msgMessage.Magnitude;
							pdTimePeriod.Offset = msgMessage.Offset;

							IRepositoryResult<bool> rsltUpdate = await repoTimePeriod.UpdateAsync(pdTimePeriod, prvTime.GetUtcNow(), tknCancellation);

							return rsltUpdate.Match(
								msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
								bResult => new MessageResult<bool>(bResult));
						});
				});
		}
	}
}