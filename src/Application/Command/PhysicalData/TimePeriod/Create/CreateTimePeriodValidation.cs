using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Validation;

namespace Application.Command.PhysicalData.TimePeriod.Create
{
	internal class CreateTimePeriodValidation : IValidation<CreateTimePeriodCommand>
	{
		async ValueTask<IMessageResult<bool>> IValidation<CreateTimePeriodCommand>.ValidateAsync(CreateTimePeriodCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);
			
			//

			return await Task.FromResult(new MessageResult<bool>(true));
		}
	}
}
