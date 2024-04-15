using Application.Interface.DataAccess;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.DataAccess
{
    internal sealed class PassportDataAccess : SqliteDataAccess, IPassportDataAccess, IDisposable
    {
        public PassportDataAccess(IConfiguration cfgConfiguration)
            : base(cfgConfiguration, "Passport")
        {

        }
    }
}
