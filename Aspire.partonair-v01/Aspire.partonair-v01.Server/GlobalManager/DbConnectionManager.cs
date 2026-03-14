//using InfrastructureLayer.partonair.Persistence;
//using Microsoft.EntityFrameworkCore;

//namespace API.partonair.GlobalManager
//{
//    public static class DbConnectionManager
//    {
//        public static IServiceCollection SqlServerConnectionManager(this IServiceCollection service, IConfiguration config)
//        {
//            string connectionString = config["ConnectionStrings:SqlServer"]
//                          ?? throw new InvalidOperationException("'ConnectionString:SqlServer' is empty.");

//            service.AddDbContext<AppDbContext>(o => o.UseSqlServer(connectionString));

//            return service;
//        }
//    }
//}
