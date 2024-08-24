using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Interface.PhysicalData;
using Application.Interface.Result;
using Application.Interface.Time;
using Domain.Interface.PhysicalData;
using Mediator;

namespace Application.Command.PhysicalData.PhysicalDimension.Create
{
    internal sealed class CreatePhysicalDimensionCommandHandler : ICommandHandler<CreatePhysicalDimensionCommand, IMessageResult<Guid>>
	{
		private readonly ITimeProvider prvTime;
		private readonly IPhysicalDimensionRepository repoPhysicalDimension;

		public CreatePhysicalDimensionCommandHandler(
			ITimeProvider prvTime,
			IPhysicalDimensionRepository repoPhysicalDimension)
		{
			this.prvTime = prvTime;
			this.repoPhysicalDimension = repoPhysicalDimension;
		}

		public async ValueTask<IMessageResult<Guid>> Handle(CreatePhysicalDimensionCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<Guid>(DefaultMessageError.TaskAborted);

			IPhysicalDimension? pdPhysicalDimension = Domain.Aggregate.PhysicalData.PhysicalDimension.PhysicalDimension.Create(
				fExponentOfAmpere: msgMessage.ExponentOfAmpere,
				fExponentOfCandela: msgMessage.ExponentOfCandela,
				fExponentOfKelvin: msgMessage.ExponentOfKelvin,
				fExponentOfKilogram: msgMessage.ExponentOfKilogram,
				fExponentOfMetre: msgMessage.ExponentOfMetre,
				fExponentOfMole: msgMessage.ExponentOfMole,
				fExponentOfSecond: msgMessage.ExponentOfSecond,
				dConversionFactorToSI: msgMessage.ConversionFactorToSI,
				sCultureName: msgMessage.CultureName,
				sName: msgMessage.Name,
				sSymbol: msgMessage.Symbol,
				sUnit: msgMessage.Unit);

			if (pdPhysicalDimension is null)
				return new MessageResult<Guid>(new MessageError() { Code = DomainError.Code.Method, Description = "Physical dimension could not be created." });

			IRepositoryResult<bool> bInsert = await repoPhysicalDimension.InsertAsync(pdPhysicalDimension, prvTime.GetUtcNow(), tknCancellation);

			return bInsert.Match(
				msgError => new MessageResult<Guid>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				bResult => new MessageResult<Guid>(pdPhysicalDimension.Id));
		}
	}
}