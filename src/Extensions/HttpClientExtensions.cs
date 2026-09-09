using CatFact.API.Client;
using CatFact.API.Models;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;

namespace CatFact.API.Extensions
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddFactResponseClient(
         this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddOptions<FactApiOptions>()
                .Bind(configuration.GetSection(FactApiOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddHttpClient<IFactResponseClient, FactResponseClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<FactApiOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
            })
            .AddResilienceHandler("fact-api-pipeline", (builder, context) =>
            {
                var options = context.ServiceProvider
                    .GetRequiredService<IOptions<FactApiOptions>>().Value;

                builder.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = options.MaxRetryAttempts,
                    BackoffType = DelayBackoffType.Exponential,
                    Delay = TimeSpan.FromMilliseconds(500),
                    UseJitter = true,
                    ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                        .Handle<HttpRequestException>()
                        .Handle<TimeoutRejectedException>()
                        .HandleResult(response => (int)response.StatusCode >= 500)
                });

                builder.AddTimeout(TimeSpan.FromSeconds(options.TimeoutSeconds));
                builder.AddTimeout(TimeSpan.FromSeconds(options.TotalTimeoutSeconds));
            });

            return services;
        }
    }
}
