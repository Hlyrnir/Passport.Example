using Application.Interface.Result;

namespace Application.Interface.Validation
{
    public interface IPassportValidation
    {
        bool IsValid { get; }

        R Match<R>(Func<IMessageError, R> MethodIfIsFailed, Func<bool, R> MethodIfIsSuccess);

        bool Add(IMessageError msgError);
        int ValidateCredential(string sCredential, string sPropertyName);
        int ValidateEmailAddress(string sEmailAddress, string sPropertyName);
        bool ValidateGuid(Guid guGuid, string sPropertyName);
        int ValidatePhoneNumber(string sPhoneNumber, string sPropertyName);
        bool ValidateProvider(string sProvider, string sPropertyName);
        int ValidateSignature(string sSignature, string sPropertyName);
    }
}