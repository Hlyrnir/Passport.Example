namespace Application.Interface.Result
{
    public interface IRepositoryResult<T>
    {
        bool IsFailed { get; }
        bool IsSuccess { get; }

        R Match<R>(Func<IRepositoryError, R> MethodIfIsFailed, Func<T, R> MethodIfIsSuccess);
        Task<R> MatchAsync<R>(Func<IRepositoryError, R> MethodIfIsFailed, Func<T, Task<R>> MethodIfIsSuccess);
    }
}