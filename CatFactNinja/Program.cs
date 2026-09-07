using CatFact.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

app.ConfigureHttpRequestPipeline();

app.Run();

// This partial class is needed for the integration tests 
public partial class Program { }