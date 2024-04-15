using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.PhysicalData;
using Application.Interface.Result;
using Domain.Interface.PhysicalData;
using Mediator;

namespace Application.Command.PhysicalData.PhysicalDimension.Delete
{
	internal sealed class DeletePhysicalDimensionCommandHandler : ICommandHandler<DeletePhysicalDimensionCommand, IMessageResult<bool>>
	{
		private readonly IPhysicalDimensionRepository repoPhysicalDimension;

		public DeletePhysicalDimensionCommandHandler(IPhysicalDimensionRepository repoPhysicalDimension)
		{
			this.repoPhysicalDimension = repoPhysicalDimension;
		}

		public async ValueTask<IMessageResult<bool>> Handle(DeletePhysicalDimensionCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			IRepositoryResult<IPhysicalDimension> rsltPhysicalDimension = await repoPhysicalDimension.FindByIdAsync(msgMessage.PhysicalDimensionId, tknCancellation);

			return await rsltPhysicalDimension.MatchAsync(
				msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				async pdPhysicalDimension =>
				{
					IRepositoryResult<bool> rsltDelete = await repoPhysicalDimension.DeleteAsync(pdPhysicalDimension, tknCancellation);

					return rsltDelete.Match(
						msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
						bResult => new MessageResult<bool>(bResult));
				});
		}
	}
}