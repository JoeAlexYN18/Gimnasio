using System.Text;
using Gimnasio.Repositories;
using Gimnasio.Services;
using Gimnasio.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace Gimnasio.Extensions
{
    /// <summary>
    /// Métodos de extensión para registrar todos los servicios de la aplicación
    /// en el contenedor de inyección de dependencias.
    /// Mantiene el Program.cs limpio y organizado.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Registra el DbContext de EF Core con el proveedor Npgsql (PostgreSQL).
        /// Lee la cadena de conexión "GymDb" desde appsettings.json.
        /// </summary>
        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddDbContext<GymDbContext>(options =>
                options.UseNpgsql(
                    config.GetConnectionString("GymDb"),
                    npgsql => npgsql.EnableRetryOnFailure()
                )
            );
            return services;
        }

        /// <summary>
        /// Registra todos los repositorios con ciclo de vida Scoped
        /// (una instancia por request HTTP).
        /// </summary>
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository,           UserRepository>();
            services.AddScoped<IRoleRepository,           RoleRepository>();
            services.AddScoped<ISocioRepository,          SocioRepository>();
            services.AddScoped<IEntrenadorRepository,     EntrenadorRepository>();
            services.AddScoped<IMembresiaRepository,      MembresiaRepository>();
            services.AddScoped<ISocioMembresiaRepository, SocioMembresiaRepository>();
            services.AddScoped<IAsistenciaRepository,     AsistenciaRepository>();
            return services;
        }

        /// <summary>
        /// Registra todos los servicios de aplicación con ciclo de vida Scoped.
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService,       AuthService>();
            services.AddScoped<ISocioService,      SocioService>();
            services.AddScoped<IEntrenadorService, EntrenadorService>();
            services.AddScoped<IMembresiaService,  MembresiaService>();
            services.AddScoped<IAsistenciaService, AsistenciaService>();
            return services;
        }

        /// <summary>
        /// Configura la autenticación JWT con HMAC-SHA256.
        /// Lee la configuración desde la sección "JwtSettings" de appsettings.json.
        ///
        /// Ejemplo de configuración en appsettings.json:
        /// <code>
        /// "JwtSettings": {
        ///   "SecretKey":          "MinimumLength32CharsSecretKeyHere!",
        ///   "Issuer":             "GymApi",
        ///   "Audience":           "GymClients",
        ///   "ExpirationMinutes":  60
        /// }
        /// </code>
        /// </summary>
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration config)
        {
            var jwt = config.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwt["SecretKey"]!);

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken            = true;
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey         = new SymmetricSecurityKey(key),
                        ValidateIssuer           = true,
                        ValidIssuer              = jwt["Issuer"],
                        ValidateAudience         = true,
                        ValidAudience            = jwt["Audience"],
                        ValidateLifetime         = true,
                        ClockSkew                = TimeSpan.Zero, 

                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.NameIdentifier
                    };
                });

            services.AddAuthorization();
            return services;
        }

        /// <summary>
        /// Configura Swagger/OpenAPI con soporte para Bearer Token.
        /// Permite probar endpoints protegidos directamente desde la UI de Swagger.
        /// </summary>
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title       = "Gym API",
                    Version     = "v1",
                    Description = "API REST para gestión de gimnasio. Autenticación JWT requerida."
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name         = "Authorization",
                    Type         = SecuritySchemeType.Http,
                    Scheme       = "Bearer",
                    BearerFormat = "JWT",
                    In           = ParameterLocation.Header,
                    Description  = "Ingrese el token JWT. Ejemplo: Bearer eyJhbGci..."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id   = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }
    }
}