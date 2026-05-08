using Infrastructure.partonair_v01.ORM.EFCore.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace Infrastructure.partonair_v01.ORM.EFCore.Factory
{
    /*
        Utilisée uniquement au design-time (migrations EF Core).
        Permet à EF Core de créer/appliquer des migrations sans le contexte
        d'injection de dépendances complet. Lit la config depuis appsettings.Development.json.
    */
    public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                                     .SetBasePath(Directory.GetCurrentDirectory())
                                     .AddJsonFile("appsettings.Development.json", optional: false)
                                     .Build();

            var cs = configuration.GetConnectionString("SqlServer_local");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(cs);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
