using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Interface.PhysicalData;
using Application.Interface.Result;
using Application.Interface.Time;
using Application.Interface.Validation;

namespace Application.Query.PhysicalData.PhysicalDimension.ById
{
    internal class PhysicalDimensionByIdValidation : IValidation<PhysicalDimensionByIdQuery>
	{
		private readonly ITimeProvider prvTime;
		private readonly IPhysicalDataValidation srvValidation;

		private readonly IPhysicalDimensionRepository repoPhysicalDimension;

		public PhysicalDimensionByIdValidation(IPhysicalDimensionRepository repoPhysicalDimension, IPhysicalDataValidation srvValidation, ITimeProvider prvTime)
		{
			this.prvTime = prvTime;
			this.srvValidation = srvValidation;

			this.repoPhysicalDimension = repoPhysicalDimension;
		}

		async ValueTask<IMessageResult<bool>> IValidation<PhysicalDimensionByIdQuery>.ValidateAsync(PhysicalDimensionByIdQuery msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			srvValidation.ValidateGuid(msgMessage.PhysicalDimensionId, "Physical dimension identifier");

			if (srvValidation.IsValid == true)
			{
				IRepositoryResult<bool> rsltPhysicalDimension = await repoPhysicalDimension.ExistsAsync(msgMessage.PhysicalDimensionId, tknCancellation);

				rsltPhysicalDimension.Match(
					msgError => srvValidation.Add(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
					bResult =>
					{
						if (bResult == false)
							srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"Physical dimension {msgMessage.PhysicalDimensionId} does not exist." });

						return bResult;
					});
			}

			return await Task.FromResult(
				srvValidation.Match(
					msgError => new MessageResult<bool>(msgError),
					bResult => new MessageResult<bool>(bResult))
				);
		}
	}
}