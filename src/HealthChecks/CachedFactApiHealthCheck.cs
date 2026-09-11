using CatFact.API.Constants;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CatFact.API.HealthChecks
{
    public class CachedFactApiHealthCheck(
        FactApiHealthCheck innerCheck,
        IMemoryCache memoryCache) : IHealthCheck
    {
        private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(30);

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            if (memoryCache.TryGetValue(CachedFactApiHealthCheckPolicies.CacheKey, out HealthCheckResult cachedResult))
            {
                return cachedResult;
            }

            var result = await innerCheck.CheckHealthAsync(context, cancellationToken);

            memoryCache.Set(CachedFactApiHealthCheckPolicies.CacheKey, result, CacheDuration);

            return result;
        }
    }
}
