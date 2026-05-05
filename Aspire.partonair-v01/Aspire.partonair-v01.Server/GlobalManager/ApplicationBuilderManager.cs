using Infrastructure.partonair_v01.ORM.EFCore.Settings;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace API.partonair_v01.GlobalManager
{
    public static class ApplicationBuilderManager
    {
        public static WebApplication ConfigureHttpPipeline(this WebApplication app)
        {
            app.UseHttpsRedirection();
            app.UseCors();
            app.UseExceptionHandler();
            app.UseStatusCodePages();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapDefaultEndpoints();
            app.MapControllers();
            app.MapOpenApi();
            app.MapScalarApiReference();
            app.MapSwagger();
            app.UseSwaggerUI();

            return app;
        }
        public static async Task<WebApplication> WaitingMigrationIsReadyAsync(this WebApplication app)
        {
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

            return app;
        }

    }
}