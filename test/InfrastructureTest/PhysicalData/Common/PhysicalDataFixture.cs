using Application.Interface.DataAccess;
using Application.Interface.PhysicalData;
using Application.Interface.UnitOfWork;
using DomainFaker;
using Infrastructure.Persistance.PhysicalData;
using Infrastructure.UnitOfWork;
using InfrastructureTest.Authorization.Common;
using InfrastructureTest.Common;
using Microsoft.Extensions.Configuration;

namespace InfrastructureTest.PhysicalData.Common
{
    public class PhysicalDataFixture
    {
        private readonly ITimeProvider prvTime;

        private readonly IConfiguration cfgConfiguration;

        private IUnitOfWork<IPhysicalDataAccess> uowUnitOfWork;

		private readonly IPhysicalDimensionRepository repoPhysicalDimension;
        private readonly ITimePeriodRepository repoTimePeriod;

        public PhysicalDataFixture()
        {
            prvTime = new TimeProviderFaker();

            cfgConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new[]
                    {
                        new KeyValuePair<string, string?>("ConnectionStrings:TestDatabase", "Data Source=D:\\Dateien\\Projekte\\CSharp\\CQRS_Prototype\\TEST_PhysicalData.db; Mode=ReadWrite")
                    })
                .Build();

            IPhysicalDataAccess sqlDataAccess = new PhysicalDataAccessFaker(cfgConfiguration, "TestDatabase");

            this.uowUnitOfWork = new UnitOfWork<IPhysicalDataAccess>(sqlDataAccess);
            this.repoPhysicalDimension = new PhysicalDimensionRepository(sqlDataAccess);
            this.repoTimePeriod = new TimePeriodRepository(sqlDataAccess);
        }

        public ITimeProvider TimeProvider { get => prvTime; }
        public IUnitOfWork<IPhysicalDataAccess> UnitOfWork { get => uowUnitOfWork; }
        public IPhysicalDimensionRepository PhysicalDimensionRepository { get => repoPhysicalDimension; }
        public ITimePeriodRepository TimePeriodRepository { get => repoTimePeriod; }
    }
}
