using API.partonair_v01.MiddlewareCustomExceptions;
using API.partonair_v01.Token;
using BLL.partonair_v01.Interfaces;
using BLL.partonair_v01.MediatR.Configurations;
using BLL.partonair_v01.Services;
using Domain.partonair_v01.Contracts;
using Infrastructure.partonair_v01.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


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

            // AUTHENTICATION
            services.AddScoped<JWTService>();


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


        public static IServiceCollection AddPresentationAPILayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container.
            services.AddControllers();

            // CORS
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin() // only for dev mod, need to specify for prod
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Problem Details format, more at https://www.rfc-editor.org/rfc/rfc9457
            services.AddProblemDetails();
            services.AddExceptionHandler<CustomExceptionHandler>();

            // Swagger/OpenAPI, more at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddOpenApi();

            // JWT Authentication
            services.Configure<JwtSettings>(configuration.GetSection("JWT")); //chope la section JWT du appsettings.json

            var jwtSettings = configuration.GetSection("JWT").Get<JwtSettings>()
                             ?? throw new InvalidOperationException("JWT settings is missing");

            var keyBytes = Convert.FromBase64String(jwtSettings.SecretKey);
            var signingKey = new SymmetricSecurityKey(keyBytes);

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // Settings for authorization policies based on roles
            services.AddAuthorizationBuilder()
                    .AddPolicy("RequireRegisterRole", policy =>
                    {
                        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                        policy.RequireAuthenticatedUser();
                        policy.RequireRole("Register"); // required role
                    })
                    .AddPolicy("RequireAdminRole", policy =>
                    {
                        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                        policy.RequireAuthenticatedUser();
                        policy.RequireRole("Admin"); // *
                    });

            return services;
        }

    }
}
