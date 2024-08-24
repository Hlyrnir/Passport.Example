using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Common.Result.Repository;
using Application.Interface.PhysicalData;
using Application.Interface.Result;
using Domain.Interface.PhysicalData;
using Mediator;

namespace Application.Query.PhysicalData.PhysicalDimension.ById
{
    internal sealed class PhysicalDimensionByIdQueryHandler : IQueryHandler<PhysicalDimensionByIdQuery, IMessageResult<PhysicalDimensionByIdResult>>
	{
		private readonly IPhysicalDimensionRepository repoPhysicalDimension;

		public PhysicalDimensionByIdQueryHandler(IPhysicalDimensionRepository repoPhysicalDimension)
		{
			this.repoPhysicalDimension = repoPhysicalDimension;
		}

		public async ValueTask<IMessageResult<PhysicalDimensionByIdResult>> Handle(PhysicalDimensionByIdQuery msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<PhysicalDimensionByIdResult>(DefaultMessageError.TaskAborted);

			RepositoryResult<IPhysicalDimension> rsltPhysicalDimension = await repoPhysicalDimension.FindByIdAsync(msgMessage.PhysicalDimensionId, tknCancellation);

			return rsltPhysicalDimension.Match(
				msgError => new MessageResult<PhysicalDimensionByIdResult>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				pdPhysicalDimension =>
				{
					PhysicalDimensionByIdResult qryResult = new PhysicalDimensionByIdResult()
					{
						PhysicalDimension = pdPhysicalDimension
					};

					return new MessageResult<PhysicalDimensionByIdResult>(qryResult);
				});
		}
	}
}