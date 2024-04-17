using Application.Interface.DataAccess;

namespace Application.Interface.UnitOfWork
{
	public interface IUnitOfWork<T> where T : IDataAccess
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="MethodForTransaction"></param>
		/// <returns></returns>
		Task TransactionAsync(Func<Task> MethodForTransaction);

		///// <summary>
		///// Open an <typeparamref name="IDbConnection"/> and initiate a database transaction.
		///// </summary>
		///// <returns>Returns <see cref="bool">true</see> if a database transaction is initiated. Otherwise, returns <see cref="bool">false</see>.</returns>
		//bool BeginTransaction();

		/// <summary>
		/// Commit the database transaction.
		/// </summary>
		/// <returns>Returns <see cref="bool">true</see> if the database transaction is successfully committed. Otherwise, returns <see cref="bool">false</see>.</returns>
		bool TryCommit();

		/// <summary>
		/// Roll back the database transaction.
		/// </summary>
		/// <returns>Returns <see cref="bool">true</see> if the database transaction is successfully rolled back. Otherwise, returns <see cref="bool">false</see>.</returns>
		bool TryRollback();
	}
}