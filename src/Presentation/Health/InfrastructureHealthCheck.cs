using Application.Interface.DataAccess;
using Application.Interface.UnitOfWork;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Presentation.Health
{
	public class InfrastructureHealthCheck : IHealthCheck
	{
		public const string Name = "Infrastructure";

		private readonly IUnitOfWork<IPassportDataAccess> uowUnitOfWork;

		public InfrastructureHealthCheck(IUnitOfWork<IPassportDataAccess> uowUnitOfWork)
        {
			this.uowUnitOfWork = uowUnitOfWork;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{
			bool bIsHealthy = false;

			await uowUnitOfWork.TransactionAsync(() =>
			{
				bIsHealthy = uowUnitOfWork.TryRollback();

				return Task.CompletedTask;
			});

			if (bIsHealthy == true)
				return HealthCheckResult.Healthy();

			return HealthCheckResult.Unhealthy("Infrastructure is unhealthy.");
		}
	}
}
