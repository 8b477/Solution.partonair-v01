using API.partonair_v01.MiddlewareCustomExceptions;
using API.partonair_v01.Token;
using BLL.partonair_v01.Interfaces;
using BLL.partonair_v01.MediatR.Configurations;
using BLL.partonair_v01.Services;
using Domain.partonair_v01.Contracts;
using Infrastructure.partonair_v01.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;

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

            // MESSAGE
            services.AddScoped<IMessageService, MessageService>();

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

            // MESSAGE
            services.AddScoped<IMessageRepository, MessageRepository>();

            // EVALUATION
            services.AddScoped<IEvaluationRepository, EvaluationRepository>();

            return services;
        }


        public static IServiceCollection AddPresentationAPILayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container.
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new System.Text.Json.Serialization.JsonStringEnumConverter()
                );
            });

            // SIGNALR (messagerie temps réel)
            services.AddSignalR();

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
                        Description = "Entrez votre token JWT, pas de préfix 'bearer' ou autre"
                    });

            // applique le schéma Bearer à tous les endpoints Swagger.
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    // référence la définition "Bearer" définie plus haut.
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            });

            services.AddOpenApi();

            // JWT Authentication
            services.Configure<JwtSettings>(configuration.GetSection("JWT")); //bind la section JWT du appsettings.json

            var jwtSettings = configuration.GetSection("JWT").Get<JwtSettings>()
                             ?? throw new InvalidOperationException("JWT settings is missing");


            if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
            {
                throw new InvalidOperationException("JWT:SecretKey is missing.");
            }

            byte[] keyBytes;

            try
            {
                keyBytes = Convert.FromBase64String(jwtSettings.SecretKey);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException(
                    "JWT:SecretKey must be a valid Base64 value.",
                    ex
                );
            }

            if (keyBytes.Length < 32)
            {
                throw new InvalidOperationException(
                    "JWT:SecretKey must decode to at least 32 bytes for HS256."
                );
            }


            //var keyBytes = Convert.FromBase64String(jwtSettings.SecretKey);
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
                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.NameIdentifier
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            // Le transport SignalR (WebSocket/SSE) ne peut pas poser l'en-tête Authorization :
                            // le client passe le token en query string ; on ne l'accepte que pour nos routes de hub.
                            var accessToken = ctx.Request.Query["access_token"];
                            if (!string.IsNullOrEmpty(accessToken) && ctx.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                            {
                                ctx.Token = accessToken;
                            }

                            var auth = ctx.Request.Headers.Authorization.ToString();
                            //Console.WriteLine($"[JWT] Authorization header: '{(string.IsNullOrEmpty(auth) ? "ABSENT" : auth[..Math.Min(40, auth.Length)] + "...")}'");
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = ctx =>
                        {
                            //Console.WriteLine($"[JWT] Auth failed: {ctx.Exception.GetType().Name} — {ctx.Exception.Message}");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = ctx =>
                        {
                            //Console.WriteLine($"[JWT] Token validated — sub: {ctx.Principal?.FindFirst("sub")?.Value}");
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
