using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CatFact.API.HealthChecks
{
    public interface IFactApiHealthCheck
    {
        Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default);
    }
}