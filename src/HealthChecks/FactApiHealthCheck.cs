using CatFact.API.Client;
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

                return HealthCheckResult.Healthy("CatFact API działa poprawnie.");
            }
            catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                return HealthCheckResult.Degraded("Żądanie do CatFact API przekroczyło limit czasu.", ex);
            }
            catch (HttpRequestException ex)
            {
                return HealthCheckResult.Degraded($"Wystąpił błąd sieci podczas komunikacji z CatFact API: {ex.Message}");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Nie udało się połączyć z CatFact API.", ex);
            }
        }
    }
}
