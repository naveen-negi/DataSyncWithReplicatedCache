using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProductPricing.API;
using SessionOrchestrator.Workflows;

var builder = WebApplication.CreateBuilder(args);


var configuration = builder.Configuration;
builder.Services.Configure<SessionServiceConfig>(configuration.GetSection("SessionService"));
builder.Services.Configure<ProductPricingServiceConfig>(configuration.GetSection("ProductPricingService"));
builder.Services.Configure<PaymentsServiceConfig>(configuration.GetSection("PaymentsService"));
builder.Services.AddScoped<ISessionWorkflow, SessionWorkflow>();

builder.Services.AddHttpLogging(o => { });


builder.Services.AddOpenTelemetry()
    .WithTracing(b => b
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("SessionOrchestrator"))
    .AddAspNetCoreInstrumentation()
    .AddHttpClientInstrumentation()
    .AddJaegerExporter(options =>
    {
        options.AgentHost = "jaeger"; // Docker service name for Jaeger
        options.AgentPort = 6831;     // Default Jaeger agent UDP port
    }));
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
app.UseHttpLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();