using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.PhysicalData;
using Application.Interface.Result;
using Application.Interface.Time;
using Domain.Interface.PhysicalData;
using Mediator;

namespace Application.Command.PhysicalData.PhysicalDimension.Update
{
	internal sealed class UpdatePhysicalDimensionCommandHandler : ICommandHandler<UpdatePhysicalDimensionCommand, IMessageResult<bool>>
	{
		private readonly ITimeProvider prvTime;
		private readonly IPhysicalDimensionRepository repoPhysicalDimension;

		public UpdatePhysicalDimensionCommandHandler(
			ITimeProvider prvTime,
			IPhysicalDimensionRepository repoPhysicalDimension)
		{
			this.prvTime = prvTime;
			this.repoPhysicalDimension = repoPhysicalDimension;
		}

		public async ValueTask<IMessageResult<bool>> Handle(UpdatePhysicalDimensionCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			IRepositoryResult<IPhysicalDimension> rsltPhysicalDimension = await repoPhysicalDimension.FindByIdAsync(msgMessage.PhysicalDimensionId, tknCancellation);

			return await rsltPhysicalDimension.MatchAsync(
				msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				async pdPhysicalDimension =>
				{
					if (pdPhysicalDimension.ConcurrencyStamp != msgMessage.ConcurrencyStamp)
						return new MessageResult<bool>(DefaultMessageError.ConcurrencyViolation);

					if (pdPhysicalDimension.TryChangeCultureName(msgMessage.CultureName) == false)
						return new MessageResult<bool>(new MessageError() { Code = DomainError.Code.Method, Description = "Culture name is not valid." });

					pdPhysicalDimension.ConversionFactorToSI = msgMessage.ConversionFactorToSI;
					pdPhysicalDimension.Name = msgMessage.Name;
					pdPhysicalDimension.Symbol = msgMessage.Symbol;
					pdPhysicalDimension.Unit = msgMessage.Unit;

					IRepositoryResult<bool> rsltUpdate = await repoPhysicalDimension.UpdateAsync(pdPhysicalDimension, prvTime.GetUtcNow(), tknCancellation);

					return rsltUpdate.Match(
						msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
						bResult => new MessageResult<bool>(bResult));
				});
		}
	}
}