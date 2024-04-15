using System.Data;

namespace Application.Interface.DataAccess
{
	public interface ISqliteDataAccess
	{
		IDbConnection Connection { get; }
		IDbTransaction? Transaction { get; }
		Task TransactionAsync(Func<Task> MethodForTransaction);
		bool TryCommit();
		bool TryRollback();
	}
}