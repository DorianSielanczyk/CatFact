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
                return HealthCheckResult.Healthy("CatFact API odpowiada poprawnie.");
            }
            catch (HttpRequestException ex)
            {
                return HealthCheckResult.Degraded($"CatFact API błąd sieci: {ex.Message}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Nie udało się połączyć z CatFact API.", ex);
            }
        }
    }
}
