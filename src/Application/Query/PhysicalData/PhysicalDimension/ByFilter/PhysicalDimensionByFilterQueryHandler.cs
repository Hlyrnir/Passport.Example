using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Common.Result.Repository;
using Application.Interface.PhysicalData;
using Application.Interface.Result;
using Domain.Interface.PhysicalData;
using Mediator;

namespace Application.Query.PhysicalData.PhysicalDimension.ByFilter
{
    internal sealed class PhysicalDimensionByFilterQueryHandler : IQueryHandler<PhysicalDimensionByFilterQuery, IMessageResult<PhysicalDimensionByFilterResult>>
	{
		private readonly IPhysicalDimensionRepository repoPhysicalDimension;

		public PhysicalDimensionByFilterQueryHandler(IPhysicalDimensionRepository repoPhysicalDimension)
		{
			this.repoPhysicalDimension = repoPhysicalDimension;
		}

		public async ValueTask<IMessageResult<PhysicalDimensionByFilterResult>> Handle(PhysicalDimensionByFilterQuery msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<PhysicalDimensionByFilterResult>(DefaultMessageError.TaskAborted);

			RepositoryResult<int> rsltQuantity = await repoPhysicalDimension.QuantityByFilterAsync(msgMessage.Filter, tknCancellation);

			return await rsltQuantity.MatchAsync(
				msgError => new MessageResult<PhysicalDimensionByFilterResult>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				async iQuantity =>
				{
					if (iQuantity < 1)
					{
						PhysicalDimensionByFilterResult qryResult = new PhysicalDimensionByFilterResult()
						{
							PhysicalDimension = Enumerable.Empty<IPhysicalDimension>(),
							MaximalNumberOfPhysicalDimension = 0
						};

						return new MessageResult<PhysicalDimensionByFilterResult>(qryResult);
					}

					RepositoryResult<IEnumerable<IPhysicalDimension>> rsltPhysicalDimension = await repoPhysicalDimension.FindByFilterAsync(msgMessage.Filter, tknCancellation);

					return rsltPhysicalDimension.Match(
						msgError => new MessageResult<PhysicalDimensionByFilterResult>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
						enumPhysicalDimension =>
						{
							PhysicalDimensionByFilterResult qryResult = new PhysicalDimensionByFilterResult()
							{
								PhysicalDimension = enumPhysicalDimension,
								MaximalNumberOfPhysicalDimension = iQuantity
							};

							return new MessageResult<PhysicalDimensionByFilterResult>(qryResult);
						});
				});
		}
	}
}