using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Validation;

namespace Application.Command.PhysicalData.PhysicalDimension.Create
{
	internal class CreatePhysicalDimensionValidation : IValidation<CreatePhysicalDimensionCommand>
	{
		async ValueTask<IMessageResult<bool>> IValidation<CreatePhysicalDimensionCommand>.ValidateAsync(CreatePhysicalDimensionCommand msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			//

			return await Task.FromResult(new MessageResult<bool>(true));
		}
	}
}
