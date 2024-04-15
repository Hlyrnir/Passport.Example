using Application.Interface.Result;

namespace Application.Common.Result.Repository
{
    public enum PassportResultState : byte
    {
        Failure,
        Success
    }

    public readonly struct RepositoryResult<T> : IRepositoryResult<T>
        where T : notnull
    {
        private readonly PassportResultState enumState;
        private readonly T? gValue;

        private readonly IRepositoryError? msgError;

        public RepositoryResult(T gValue)
        {
            enumState = PassportResultState.Success;
            this.gValue = gValue;
            msgError = null;
        }

        public RepositoryResult(IRepositoryError msgError)
        {
            enumState = PassportResultState.Failure;
            gValue = default;
            this.msgError = msgError;
        }

        public static implicit operator RepositoryResult<T>(T gValue)
        {
            return new RepositoryResult<T>(gValue);
        }

        public bool IsFailed
        {
            get
            {
                if (enumState == PassportResultState.Failure)
                    return true;

                return false;
            }
        }

        public bool IsSuccess
        {
            get
            {
                if (enumState == PassportResultState.Success)
                    return true;

                return false;
            }
        }

        public R Match<R>(Func<IRepositoryError, R> MethodIfIsFailed, Func<T, R> MethodIfIsSuccess)
        {
            if (MethodIfIsSuccess is null || MethodIfIsFailed is null)
                throw new NotImplementedException("Match function is not defined.");

            if (gValue is null && msgError is null)
                throw new InvalidOperationException("No result was found.");

            if (enumState == PassportResultState.Success)
                return MethodIfIsSuccess(gValue!);

            return MethodIfIsFailed(msgError!);
        }

        public async Task<R> MatchAsync<R>(Func<IRepositoryError, R> MethodIfIsFailed, Func<T, Task<R>> MethodIfIsSuccess)
        {
            if (MethodIfIsSuccess is null || MethodIfIsFailed is null)
                throw new NotImplementedException("Match function is not defined.");

            if (gValue is null && msgError is null)
                throw new InvalidOperationException("No result was found.");

            if (enumState == PassportResultState.Success)
                return await MethodIfIsSuccess(gValue!);

            return MethodIfIsFailed(msgError!);
        }
    }
}