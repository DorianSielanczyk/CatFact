using CatFact.API.Constants;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace CatFact.API.Extensions
{
    public static class RateLimiterExtensions
    {
        public static IServiceCollection AddApplicationRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                    await context.HttpContext.Response.WriteAsync("Zbyt wiele żądań. Proszę spróbować ponownie później.", cancellationToken);
                };

                options.AddFixedWindowLimiter(policyName: RateLimitPolicies.PostFactPolicy, limiterOptions =>
                {
                    limiterOptions.PermitLimit = 5; 
                    limiterOptions.Window = TimeSpan.FromSeconds(10); 
                    limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    limiterOptions.QueueLimit = 0; 
                });
            });

            return services;
        }
    }
}
