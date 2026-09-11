namespace CatFact.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication ConfigureHttpRequestPipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapControllers();
            app.MapHealthChecks("/health");

            return app;
        }
    }
}
