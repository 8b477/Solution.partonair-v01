using API.partonair_v01.GlobalManager;
using Infrastructure.partonair_v01.ORM.EFCore.Settings;


var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddSqlServerDbContext<ApplicationDbContext>("partonairdb");
builder.AddRedisOutputCache("cache");

// Dependency Injection
builder.Services.AddPresentationAPILayer()
                .AddApplicationLayer()
                .AddInfrastructureLayer();


var app = builder.Build();

app.WaitingMigrationIsReadyAsync().Wait();

app.ConfigureHttpPipeline()
   .UseOutputCache()
   .UseFileServer();

app.Run();
