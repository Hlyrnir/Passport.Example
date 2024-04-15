using Application.Interface.DataAccess;
using Application.Interface.UnitOfWork;

namespace ApplicationTest.InfrastructureFaker
{
	internal class PassportUnitOfWorkFaker : IUnitOfWork<IPassportDataAccess>
	{
		public PassportUnitOfWorkFaker()
		{

		}

		public async Task TransactionAsync(Func<Task> MethodForTransaction)
		{
			await MethodForTransaction();
		}

		public bool TryCommit()
		{
			return true;
		}

		public bool TryRollback()
		{
			return true;
		}
	}

	internal class PhysicalDataUnitOfWorkFaker : IUnitOfWork<IPhysicalDataAccess>
	{
		public PhysicalDataUnitOfWorkFaker()
		{

		}

		public async Task TransactionAsync(Func<Task> MethodForTransaction)
		{
			await MethodForTransaction();
		}

		public bool TryCommit()
		{
			return true;
		}

		public bool TryRollback()
		{
			return true;
		}
	}
}
