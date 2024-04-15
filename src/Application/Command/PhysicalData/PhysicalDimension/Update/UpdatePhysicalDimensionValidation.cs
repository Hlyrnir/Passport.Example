using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.PhysicalData;
using Application.Interface.Result;
using Application.Interface.Validation;

namespace Application.Command.PhysicalData.PhysicalDimension.Update
{
	internal class UpdatePhysicalDimensionValidation : IValidation<UpdatePhysicalDimensionCommand>
	{
		private readonly IPhysicalDataValidation srvValidation;
		private readonly IPhysicalDimensionRepository repoPhysicalDimension;

		public UpdatePhysicalDimensionValidation(IPhysicalDataValidation srvValidation, IPhysicalDimensionRepository repoPhysicalDimension)
		{
			this.srvValidation = srvValidation;
			this.repoPhysicalDimension = repoPhysicalDimension;
		}

		async ValueTask<IMessageResult<bool>> IValidation<UpdatePhysicalDimensionCommand>.ValidateAsync(UpdatePhysicalDimensionCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			IRepositoryResult<bool> rsltPhysicalDimension = await repoPhysicalDimension.ExistsAsync(msgMessage.PhysicalDimensionId, tknCancellation);

			rsltPhysicalDimension.Match(
				msgError => srvValidation.Add(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				bResult =>
				{
					if (bResult == false)
						srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"Physical dimension {msgMessage.PhysicalDimensionId} does not exist." });

					return bResult;
				});

			return srvValidation.Match(
				msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				bResult => new MessageResult<bool>(bResult));
		}
	}
}
