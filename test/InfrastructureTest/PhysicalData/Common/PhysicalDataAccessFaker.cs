using Application.Interface.DataAccess;
using Infrastructure.DataAccess;
using Microsoft.Extensions.Configuration;

namespace InfrastructureTest.PhysicalData.Common
{
    internal sealed class PhysicalDataAccessFaker : SqliteDataAccess, IPhysicalDataAccess, IDisposable
    {
        public PhysicalDataAccessFaker(IConfiguration cfgConfiguration, string sConnectionStringName = "Default")
            : base(cfgConfiguration, sConnectionStringName)
        {

        }
    }
}