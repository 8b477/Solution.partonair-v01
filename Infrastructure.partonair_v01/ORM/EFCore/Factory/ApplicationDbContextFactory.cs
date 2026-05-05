using Infrastructure.partonair_v01.ORM.EFCore.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace Infrastructure.partonair_v01.ORM.EFCore.Factory
{
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
