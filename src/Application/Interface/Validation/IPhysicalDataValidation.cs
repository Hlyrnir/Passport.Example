using Application.Interface.Result;

namespace Application.Interface.Validation
{
    public interface IPhysicalDataValidation
    {
        bool IsValid { get; }
        bool Add(IMessageError msgError);
        R Match<R>(Func<IMessageError, R> MethodIfIsFailed, Func<bool, R> MethodIfIsSuccess);

        bool ValidateGuid(Guid guGuid, string sPropertyName);
        bool ValidateAgainstSqlInjection(string sString, string sPropertyName);
	}
}