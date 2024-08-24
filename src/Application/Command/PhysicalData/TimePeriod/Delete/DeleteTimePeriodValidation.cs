using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Interface.PhysicalData;
using Application.Interface.Result;
using Application.Interface.Validation;

namespace Application.Command.PhysicalData.TimePeriod.Delete
{
    internal class DeleteTimePeriodValidation : IValidation<DeleteTimePeriodCommand>
	{
		private readonly IPhysicalDataValidation srvValidation;
		private readonly ITimePeriodRepository repoTimePeriod;

		public DeleteTimePeriodValidation(
			IPhysicalDataValidation srvValidation,
			ITimePeriodRepository repoTimePeriod)
		{
			this.srvValidation = srvValidation;
			this.repoTimePeriod = repoTimePeriod;
		}

		async ValueTask<IMessageResult<bool>> IValidation<DeleteTimePeriodCommand>.ValidateAsync(DeleteTimePeriodCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			IRepositoryResult<bool> rsltTimePeriod = await repoTimePeriod.ExistsAsync(msgMessage.TimePeriodId, tknCancellation);

			rsltTimePeriod.Match(
				msgError => srvValidation.Add(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				bResult =>
				{
					if (bResult == false)
						srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"Time period {msgMessage.TimePeriodId} does not exist." });

					return bResult;
				});

			return srvValidation.Match(
				msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				bResult => new MessageResult<bool>(bResult));
		}
	}
}
