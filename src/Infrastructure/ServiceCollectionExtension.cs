using Application.Interface.DataAccess;
using Application.Interface.Passport;
using Application.Interface.PhysicalData;
using Application.Interface.UnitOfWork;
using Infrastructure.DataAccess;
using Infrastructure.Persistance.Authorization;
using Infrastructure.Persistance.PhysicalData;
using Infrastructure.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection cltService, IConfiguration cfgConfiguration)
		{
			// Add data access
			cltService.TryAddScoped<IPassportDataAccess, PassportDataAccess>();
			cltService.TryAddScoped<IPhysicalDataAccess, PhysicalDataAccess>();

			// Add unit of work
			cltService.TryAddTransient<IUnitOfWork<IPassportDataAccess>, UnitOfWork<IPassportDataAccess>>();
			cltService.TryAddTransient<IUnitOfWork<IPhysicalDataAccess>, UnitOfWork<IPhysicalDataAccess>>();

			// Add passport repository
			cltService.TryAddTransient<IPassportHolderRepository, PassportHolderRepository>();
			cltService.TryAddTransient<IPassportRepository, PassportRepository>();
			cltService.TryAddTransient<IPassportTokenRepository, PassportTokenRepository>();
			cltService.TryAddTransient<IPassportVisaRepository, PassportVisaRepository>();

			// Add business repository
			cltService.AddTransient<IPhysicalDimensionRepository, PhysicalDimensionRepository>();
			cltService.AddTransient<ITimePeriodRepository, TimePeriodRepository>();

			return cltService;
		}
	}
}