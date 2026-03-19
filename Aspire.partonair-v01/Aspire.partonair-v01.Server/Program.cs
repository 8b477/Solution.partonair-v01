using API.partonair_v01.GlobalManager;
using Infrastructure.partonair_v01.ORM.EFCore.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddSqlServerDbContext<ApplicationDbContext>("partonairdb");
builder.AddRedisOutputCache("cache");
builder.Services.AddProblemDetails();
builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddApplicationLayer()
                .AddInfrastructureLayer()
                .AddPresentationAPILayer();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options => {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "ton-issuer",
            ValidAudience = "ton-audience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ta-cle-super-secrete-min-256bits"))
        };
    });

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Partonair API", Version = "v1" });
});


var app = builder.Build();

await Task.Delay(10000);

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.SetCommandTimeout(300);  // 5 minutes

    int retries = 3;
    for (int i = 0; i < retries; i++)
    {
        try
        {
            await context.Database.MigrateAsync();
            break;
        }
        catch (SqlException) when (i < retries - 1)
        {
            await Task.Delay(5000); // 5 secondes d’attente
        }
    }
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Partonair API v1");
    });
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseOutputCache();

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

var api = app.MapGroup("/api");



// WeatherForecast (cache Redis)
api.MapGet("weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.CacheOutput(p => p.Expire(TimeSpan.FromSeconds(5)))
.WithName("GetWeatherForecast");


app.MapDefaultEndpoints();
app.UseFileServer();
app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


