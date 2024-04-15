using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.PhysicalData;
using Application.Interface.Result;
using Application.Interface.Validation;

namespace Application.Command.PhysicalData.TimePeriod.Update
{
	internal class UpdateTimePeriodValidation : IValidation<UpdateTimePeriodCommand>
	{
		private readonly IPhysicalDataValidation srvValidation;
		private readonly IPhysicalDimensionRepository repoPhysicalDimension;
		private readonly ITimePeriodRepository repoTimePeriod;

		public UpdateTimePeriodValidation(
			IPhysicalDataValidation srvValidation,
			IPhysicalDimensionRepository repoPhysicalDimension, 
			ITimePeriodRepository repoTimePeriod)
		{
			this.srvValidation = srvValidation;
			this.repoPhysicalDimension = repoPhysicalDimension;
			this.repoTimePeriod = repoTimePeriod;
		}

		async ValueTask<IMessageResult<bool>> IValidation<UpdateTimePeriodCommand>.ValidateAsync(UpdateTimePeriodCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			srvValidation.ValidateGuid(msgMessage.PhysicalDimensionId, "Physical dimension identifer");
			srvValidation.ValidateGuid(msgMessage.TimePeriodId, "Time period identifier");

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

				IRepositoryResult<bool> rsltTimePeriod = await repoTimePeriod.ExistsAsync(msgMessage.TimePeriodId, tknCancellation);

				rsltTimePeriod.Match(
					msgError => srvValidation.Add(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
					bResult =>
					{
						if (bResult == false)
							srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"Time period {msgMessage.TimePeriodId} does not exist." });

						return bResult;
					});
			}

			return srvValidation.Match(
				msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				bResult => new MessageResult<bool>(bResult));
		}
	}
}
