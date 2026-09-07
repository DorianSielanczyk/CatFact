using CatFact.API.Client;

namespace CatFact.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            var catFactApiUrl = configuration["CatFactApi:BaseUrl"];

            services.AddHttpClient<IFactResponseClient, FactResponseClient>(client =>
            {
                client.BaseAddress = new Uri(catFactApiUrl!);
            });

            return services;
        }
    }
}
