using ProductPricing.API;
using SessionOrchestrator.Workflows;

var builder = WebApplication.CreateBuilder(args);


var configuration = builder.Configuration;
builder.Services.Configure<SessionServiceConfig>(configuration.GetSection("SessionService"));
builder.Services.AddScoped<ISessionWorkflow, SessionWorkflow>();

builder.Services.AddHttpLogging(o => { });
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