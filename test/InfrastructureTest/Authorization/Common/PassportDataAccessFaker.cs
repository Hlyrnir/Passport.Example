using Application.Interface.DataAccess;
using Infrastructure.DataAccess;
using Microsoft.Extensions.Configuration;

namespace InfrastructureTest.Authorization.Common
{
	internal sealed class PassportDataAccessFaker : SqliteDataAccess, IPassportDataAccess, IDisposable
	{
		public PassportDataAccessFaker(IConfiguration cfgConfiguration, string sConnectionStringName = "Default")
			: base(cfgConfiguration, sConnectionStringName)
		{

		}
	}
}
