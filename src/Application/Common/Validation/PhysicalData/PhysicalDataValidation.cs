using Application.Common.Error;
using Application.Common.Result.Message;
using Application.Common.Validation.Message;
using Application.Interface.Validation;

namespace Application.Common.Validation.PhysicalData
{
    internal sealed class PhysicalDataValidation : MessageValidation, IPhysicalDataValidation
	{
		public bool ValidateGuid(Guid guGuid, string sPropertyName)
		{
			if (Equals(guGuid, Guid.Empty) == true)
			{
				Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"{sPropertyName} is invalid (empty)." });
				return false;
			}

			return true;
		}

		public bool ValidateAgainstSqlInjection(string sString, string sPropertyName)
		{
			if (sString.Contains("--", StringComparison.InvariantCultureIgnoreCase) == true
				|| sString.Contains("ALTER", StringComparison.InvariantCultureIgnoreCase) == true
				|| sString.Contains("DELETE", StringComparison.InvariantCultureIgnoreCase) == true
				|| sString.Contains("DROP", StringComparison.InvariantCultureIgnoreCase) == true
				|| sString.Contains("INSERT", StringComparison.InvariantCultureIgnoreCase) == true
				|| sString.Contains("SELECT", StringComparison.InvariantCultureIgnoreCase) == true
				|| sString.Contains("UPDATE", StringComparison.InvariantCultureIgnoreCase) == true)
			{
				Add(new MessageError() { Code = ValidationError.Code.Method, Description = $"{sPropertyName} contains forbidden statement." });
				return false;
			}

			return true;
		}
	}
}