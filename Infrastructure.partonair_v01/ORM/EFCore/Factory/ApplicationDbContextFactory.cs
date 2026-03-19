//using Infrastructure.partonair_v01.ORM.EFCore.Settings;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;


//namespace Infrastructure.partonair_v01.ORM.EFCore.Factory
//{
//    public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
//    {
//        public ApplicationDbContext CreateDbContext(string[] args)
//        {
//            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

//            var configuration = new ConfigurationBuilder()
//                                     .SetBasePath(Directory.GetCurrentDirectory())
//                                     //.AddJsonFile("appsettings.json", optional: false)
//                                     .AddJsonFile("appsettings.Development.json", optional: false)
//                                     .Build();

//            var cs = configuration.GetConnectionString("partonairdb");

//            optionsBuilder.UseSqlServer();

//            return new ApplicationDbContext(optionsBuilder.Options);
//        }
//    }
//}