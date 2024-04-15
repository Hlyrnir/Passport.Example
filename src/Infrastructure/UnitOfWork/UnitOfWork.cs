using Application.Interface.DataAccess;
using Application.Interface.UnitOfWork;

namespace Infrastructure.UnitOfWork
{
	internal sealed class UnitOfWork<T> : IUnitOfWork<T> where T : ISqliteDataAccess
	{
		private readonly ISqliteDataAccess sqlDataAccess;

		public UnitOfWork(T sqlDataAccess)
		{
			this.sqlDataAccess = sqlDataAccess;
		}

		/// <inheritdoc/>
		public async Task TransactionAsync(Func<Task> MethodForTransaction)
		{
			await sqlDataAccess.TransactionAsync(MethodForTransaction);
		}

		/// <inheritdoc/>
		public bool TryCommit()
		{
			return sqlDataAccess.TryCommit();
		}

		/// <inheritdoc/>
		public bool TryRollback()
		{
			return sqlDataAccess.TryRollback();
		}
	}
}