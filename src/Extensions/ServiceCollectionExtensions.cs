using CatFact.API.Clients;
using CatFact.API.Exceptions;
using CatFact.API.HealthChecks;
using CatFact.API.Models;
using CatFact.API.Services;

namespace CatFact.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            services.AddFactResponseClient(configuration);

            services.AddMemoryCache();

            services.AddTransient<FactApiHealthCheck>();
            services.AddTransient<CachedFactApiHealthCheck>();

            services.AddApplicationRateLimiting();

            services.AddHealthChecks()
               .AddCheck<CachedFactApiHealthCheck>("catfact-api", tags: ["external"]);

            services.Configure<FileStorageOptions>(configuration.GetSection("FileStorage"));

            services.AddScoped<IFactResponseService, FactResponseService>();

            return services;
        }
    }
}
