using Application.Common.Validation.PhysicalData;
using Application.Interface.DataAccess;
using Application.Interface.Passport;
using Application.Interface.PhysicalData;
using Application.Interface.Time;
using Application.Interface.UnitOfWork;
using Application.Interface.Validation;
using ApplicationTest.InfrastructureFaker;
using ApplicationTest.InfrastructureFaker.PhysicalData;

namespace ApplicationTest.Common
{
	public sealed class PhysicalDataFixture
	{
		private readonly IPhysicalDimensionRepository repoPhysicalDimension;
		private readonly ITimePeriodRepository repoTimePeriod;

		private readonly IUnitOfWork<IPhysicalDataAccess> uowUnitOfWork;

		private readonly PassportFixture fxtAuthorization;

		private readonly PhysicalDatabaseFaker dbFaker;

		public PhysicalDataFixture()
		{
			fxtAuthorization = new PassportFixture();

			uowUnitOfWork = new PhysicalDataUnitOfWorkFaker();

			dbFaker = new PhysicalDatabaseFaker();

			repoPhysicalDimension = new PhysicalDimensionRepositoryFaker(dbFaker);
			repoTimePeriod = new TimePeriodRepositoryFaker(dbFaker);
		}

		public ITimeProvider TimeProvider { get => fxtAuthorization.TimeProvider; }
		public IPassportVisaRepository PassportVisaRepository { get => fxtAuthorization.PassportVisaRepository; }
		public IPhysicalDimensionRepository PhysicalDimensionRepository { get => repoPhysicalDimension; }
		public ITimePeriodRepository TimePeriodRepository { get => repoTimePeriod; }
		public IUnitOfWork<IPhysicalDataAccess> UnitOfWork { get => uowUnitOfWork; }
		public IPhysicalDataValidation PhysicalDataValiation { get => new PhysicalDataValidation(); }

	}
}