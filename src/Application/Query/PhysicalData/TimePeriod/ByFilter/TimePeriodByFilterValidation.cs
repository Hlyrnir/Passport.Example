using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Interface.Result;
using Application.Interface.Validation;

namespace Application.Query.PhysicalData.TimePeriod.ByFilter
{
    internal class TimePeriodByFilterValidation : IValidation<TimePeriodByFilterQuery>
	{
		private readonly IPhysicalDataValidation srvValidation;

		public TimePeriodByFilterValidation(IPhysicalDataValidation srvValidation)
		{
			this.srvValidation = srvValidation;
		}

		async ValueTask<IMessageResult<bool>> IValidation<TimePeriodByFilterQuery>.ValidateAsync(TimePeriodByFilterQuery msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			if (msgMessage.Filter.Page <= 0)
				srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = "Page has to be greater than zero." });

			if (msgMessage.Filter.PageSize <= 0)
				srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = "Page size has to be greater than zero." });

			return await Task.FromResult(srvValidation.Match(
				msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				bResult => new MessageResult<bool>(bResult)));
		}
	}
}