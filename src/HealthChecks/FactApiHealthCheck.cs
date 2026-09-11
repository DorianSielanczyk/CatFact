using CatFact.API.Clients;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CatFact.API.HealthChecks
{
    public class FactApiHealthCheck(IFactResponseClient factClient) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await factClient.GetFactResponseAsync(cancellationToken);
                return HealthCheckResult.Healthy("CatFact API is responding correctly.");
            }
            catch (HttpRequestException ex)
            {
                return HealthCheckResult.Degraded($"CatFact API network error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Failed to connect to CatFact API.", ex);
            }
        }
    }
}
