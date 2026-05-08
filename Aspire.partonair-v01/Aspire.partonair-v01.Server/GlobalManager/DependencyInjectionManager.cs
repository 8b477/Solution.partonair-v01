using API.partonair_v01.MiddlewareCustomExceptions;
using API.partonair_v01.Token;
using BLL.partonair_v01.Interfaces;
using BLL.partonair_v01.MediatR.Configurations;
using BLL.partonair_v01.Services;
using Domain.partonair_v01.Contracts;
using Infrastructure.partonair_v01.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Reflection;


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

            // HTTP ACCESS
            services.AddHttpContextAccessor();
            services.AddScoped<IHttpContextAccesor, HttpContextAccesor>();


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
            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    // Préserve le Detail déjà défini par le CustomExceptionHandler
                    if (context.Exception is not null && string.IsNullOrEmpty(context.ProblemDetails.Detail))
                    {
                        context.ProblemDetails.Detail = context.Exception.Message;
                    }
                };
            });
            services.AddExceptionHandler<CustomExceptionHandler>();
            services.AddExceptionHandler<CustomExceptionHandler>();

            // Swagger/OpenAPI, more at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                // Récupère le chemin du fichier XML généré
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

                options.IncludeXmlComments(xmlPath);

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Entrez votre token JWT, uniquement la valeur de celui-ci pas de préfix"
                });

            });
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
                        ClockSkew = TimeSpan.Zero,
                        RoleClaimType = "role",
                        NameClaimType = "sub"
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            var auth = ctx.Request.Headers.Authorization.ToString();
                            Console.WriteLine($"[JWT] Authorization header: '{(string.IsNullOrEmpty(auth) ? "ABSENT" : auth[..Math.Min(40, auth.Length)] + "...")}'");
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = ctx =>
                        {
                            Console.WriteLine($"[JWT] Auth failed: {ctx.Exception.GetType().Name} — {ctx.Exception.Message}");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = ctx =>
                        {
                            Console.WriteLine($"[JWT] Token validated — sub: {ctx.Principal?.FindFirst("sub")?.Value}");
                            return Task.CompletedTask;
                        }
                    };
                });

            // Settings for authorization policies based on roles
            services.AddAuthorizationBuilder()
                    .AddPolicy("RequireVisitorRole", policy =>
                    {
                        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                        policy.RequireAuthenticatedUser();
                        policy.RequireRole("Visitor"); // required role 'Visitor'
                    })
                    .AddPolicy("RequireEmployeeRole", policy =>
                    {
                        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                        policy.RequireAuthenticatedUser();
                        policy.RequireRole("Employee"); // required role 'Employee'
                    })
                    .AddPolicy("RequireCompanyRole", policy =>
                    {
                        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                        policy.RequireAuthenticatedUser();
                        policy.RequireRole("Company"); // required role 'Company'
                    })
                    .AddPolicy("RequireAdminRole", policy =>
                    {
                        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                        policy.RequireAuthenticatedUser();
                        policy.RequireRole("Admin"); // required role 'Admin'
                    })
                    .AddPolicy("RequireMustRole", policy =>
                    {
                        policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                        policy.RequireAuthenticatedUser();
                        policy.RequireRole("Visitor","Employee","Company","Admin"); // required role one of role
                    });

            return services;
        }

    }
}
