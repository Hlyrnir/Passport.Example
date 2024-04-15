using Application.Interface.DataAccess;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.DataAccess
{
	internal sealed class PhysicalDataAccess : SqliteDataAccess, IPhysicalDataAccess
	{
		public PhysicalDataAccess(IConfiguration cfgConfiguration)
			: base(cfgConfiguration, "PhysicalData")
		{

		}
	}
}
