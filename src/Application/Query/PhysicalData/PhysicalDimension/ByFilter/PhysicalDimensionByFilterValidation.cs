using Application.Common.Result.Message;
using Application.Error;
using Application.Interface.Result;
using Application.Interface.Validation;

namespace Application.Query.PhysicalData.PhysicalDimension.ByFilter
{
	internal class PhysicalDimensionByFilterValidation : IValidation<PhysicalDimensionByFilterQuery>
	{
		private readonly IPhysicalDataValidation srvValidation;

        public PhysicalDimensionByFilterValidation(IPhysicalDataValidation srvValidation)
        {
			this.srvValidation = srvValidation;
		}

		async ValueTask<IMessageResult<bool>> IValidation<PhysicalDimensionByFilterQuery>.ValidateAsync(PhysicalDimensionByFilterQuery msgMessage, CancellationToken tknCancellation)
		{
			if (tknCancellation.IsCancellationRequested)
				return new MessageResult<bool>(DefaultMessageError.TaskAborted);

			if (msgMessage.Filter.Page <= 0)
				srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = "Page has to be greater than zero." });

			if (msgMessage.Filter.PageSize <= 0)
				srvValidation.Add(new MessageError() { Code = ValidationError.Code.Method, Description = "Page size has to be greater than zero." });

			if (msgMessage.Filter.CultureName is not null)
				srvValidation.ValidateAgainstSqlInjection(msgMessage.Filter.CultureName, "Culture name");

			if (msgMessage.Filter.Name is not null)
				srvValidation.ValidateAgainstSqlInjection(msgMessage.Filter.Name, "Name");

			if (msgMessage.Filter.Symbol is not null)
				srvValidation.ValidateAgainstSqlInjection(msgMessage.Filter.Symbol, "Symbol");

			if (msgMessage.Filter.Unit is not null)
				srvValidation.ValidateAgainstSqlInjection(msgMessage.Filter.Unit, "Unit");

			return await Task.FromResult(srvValidation.Match(
				msgError => new MessageResult<bool>(new MessageError() { Code = msgError.Code, Description = msgError.Description }),
				bResult => new MessageResult<bool>(bResult)));
		}
	}
}