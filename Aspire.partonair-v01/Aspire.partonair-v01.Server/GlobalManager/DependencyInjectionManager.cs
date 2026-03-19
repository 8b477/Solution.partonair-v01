using API.partonair_v01.MiddlewareCustomExceptions;
using BLL.partonair_v01.Interfaces;
using BLL.partonair_v01.MediatR.Configurations;
using BLL.partonair_v01.Services;
using Domain.partonair_v01.Contracts;
using Infrastructure.partonair_v01.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace API.partonair_v01.GlobalManager
{
    public static class DependencyInjectionManager
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            // MEDIATR
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(typeof(ApplicationLayerMediatRConfiguration).Assembly);
            });

            // USER
            services.AddScoped<IUserService, UserService>();

            // PROFILE
            services.AddScoped<IProfileService, ProfileService>();

            // CONTACT
            services.AddScoped<IContactService, ContactService>();

            // EVALUATION
            services.AddScoped<IEvaluationService, EvaluationService>();

            // UNIT OF WORK
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // BCrypt
            services.AddScoped<IBCryptService, BCryptService>();


            return services;
        }

        
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services)
        {
            // USER
            services.AddScoped<IUserRepository, UserRepository>();

            //PROFILE
            services.AddScoped<IProfileRepository, ProfileRepository>();

            // CONTACT
            services.AddScoped<IContactRepository, ContactRepository>();

            // EVALUATION
            services.AddScoped<IEvaluationRepository, EvaluationRepository>();

            return services;
        }


        public static IServiceCollection AddPresentationAPILayer(this IServiceCollection services)
        {
            // Add services to the container.
            services.AddControllers();

            // Problem Details format, more at https://www.rfc-editor.org/rfc/rfc9457
            services.AddProblemDetails();
            services.AddExceptionHandler<CustomExceptionHandler>();

            // Swagger/OpenAPI, more at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddOpenApi();

            // JWT Authentication
            services.AddAuthentication("Bearer")
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

            return services;
        }

    }
}
