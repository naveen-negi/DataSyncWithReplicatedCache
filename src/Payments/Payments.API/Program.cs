using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Payments.API;
using Payments.API.Repositories;
using Payments.API.Services;
using ProductPricing.API.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
{
 builder.Services.AddDbContext<PaymentsDBContext>(options =>
 {
     options.UseNpgsql(builder.Configuration.GetConnectionString("PaymentsDB"));
 });

    // builder.Services.AddRefitClient<IProductPricingApiClient>();
    // builder.Services.AddScoped<ISessionApiClient>();
    
    builder.Services.AddScoped<IStripeApiClient, MockStripeApiClient>();
    builder.Services.AddScoped<IPaymentsService, PaymentsService>();
    builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
    builder.Services.AddScoped<IUserPaymentDetailsRepository, UserPaymentDetailsRepository>();
    var configuration = builder.Configuration;
    
    builder.Services.Configure<SessionServiceConfig>(configuration.GetSection("SessionService"));
    builder.Services.Configure<ProductPricingServiceConfig>(configuration.GetSection("ProductPricingService"));
    
builder.Services.AddOpenTelemetry()
    .WithTracing(b => b
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Payments"))
    .AddAspNetCoreInstrumentation()
    .AddHttpClientInstrumentation()
    .AddJaegerExporter(options =>
    {
        options.AgentHost = "jaeger"; // Docker service name for Jaeger
        options.AgentPort = 6831;     // Default Jaeger agent UDP port
    }));
}

builder.Services.AddControllers();

builder.Services.AddHttpLogging(o => { });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<PaymentsDBContext>();

    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Log or handle the exception as needed
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseHttpLogging();

app.MapControllers();

app.UseHttpLogging();
app.Run();
