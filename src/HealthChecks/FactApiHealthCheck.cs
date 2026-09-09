using CatFact.API.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace CatFact.API.HealthChecks
{
    public class FactApiHealthCheck(IHttpClientFactory httpClientFactory, IOptions<FactApiOptions> options)
        : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var client = httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(3);

                var response = await client.GetAsync(
                    new Uri(new Uri(options.Value.BaseUrl), "fact"),
                    cancellationToken);

                return response.IsSuccessStatusCode
                    ? HealthCheckResult.Healthy("CatFact API odpowiada poprawnie.")
                    : HealthCheckResult.Degraded($"CatFact API zwróciło status {(int)response.StatusCode}.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Brak połączenia z CatFact API.", ex);
            }
        }
    }
}
