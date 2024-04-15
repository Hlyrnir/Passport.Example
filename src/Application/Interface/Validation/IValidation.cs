using Application.Interface.Result;

namespace Application.Interface.Validation
{
    public interface IValidation<in T>
    {
        ValueTask<IMessageResult<bool>> ValidateAsync(T msgMessage, CancellationToken tknCancellation);
    }
}
