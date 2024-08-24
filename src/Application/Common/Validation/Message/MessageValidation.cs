using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Interface.Result;
using Application.Interface.Validation;
using System.Text.Json;

namespace Application.Common.Validation.Message
{
    public class MessageValidation : IMessageValidation
	{
		private List<IMessageError> lstValidationError;

		public MessageValidation()
		{
			lstValidationError = new List<IMessageError>();
		}

		public bool IsValid
		{
			get
			{
				if (lstValidationError.Count == 0)
					return true;

				return false;
			}
		}

		public R Match<R>(Func<IMessageError, R> MethodIfIsFailed, Func<bool, R> MethodIfIsSuccess)
		{
			if (MethodIfIsSuccess is null || MethodIfIsFailed is null)
				throw new NotImplementedException("Match function is not defined.");

			if (IsValid)
				return MethodIfIsSuccess(true);

			return MethodIfIsFailed(Summary());
		}

		public bool Add(IMessageError msgError)
		{
			if (msgError is null)
				return false;

			lstValidationError.Add(msgError);

			return true;
		}

		private IMessageError Summary()
		{
			JsonSerializerOptions jsonOption = new JsonSerializerOptions()
			{
				Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
				WriteIndented = false
			};

			return new MessageError()
			{
				Code = ValidationError.Code.Method,
				Description = JsonSerializer.Serialize(lstValidationError, jsonOption)
			};
		}
	}
}