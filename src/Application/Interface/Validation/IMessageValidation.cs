using Application.Interface.Result;

namespace Application.Interface.Validation
{
    public interface IMessageValidation
    {
        bool IsValid { get; }

        bool Add(IMessageError msgError);
        R Match<R>(Func<IMessageError, R> MethodIfIsFailed, Func<bool, R> MethodIfIsSuccess);
    }
}