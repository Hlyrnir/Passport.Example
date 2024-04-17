using System.Data;

namespace Application.Interface.DataAccess
{
	public interface IDataAccess
	{
		IDbConnection Connection { get; }
		IDbTransaction? Transaction { get; }
		Task TransactionAsync(Func<Task> MethodForTransaction);
		bool TryCommit();
		bool TryRollback();
	}
}