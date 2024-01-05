using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using TodoLibrary.Data;
using TodoLibrary.DataAccess;

namespace TodoApi.StartupConfig
{
    public static class DependencyInjectionExtensions
    {
        public static void AddStandardServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.AddSwaggerServices();
        }

        private static void AddSwaggerServices(this WebApplicationBuilder builder)
        {
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT Authorization header using the Bearer scheme.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            };

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "BearerAuth",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new String[] { }
                }
            };

            builder.Services.AddSwaggerGen(opts =>
            {
                opts.AddSecurityDefinition("BearerAuth", securityScheme);
                opts.AddSecurityRequirement(securityRequirement);
                opts.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Our user API",
                    Version = "v1",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Taylor Maxwell",
                        Email = "taylor@imaxwell.net",
                        Url = new Uri("https://imaxwell.net"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under MIT",
                        Url = new Uri("https://example.com/license"),
                    }
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
            });
        }

        public static void AddVersioning(this WebApplicationBuilder builder)
        {
            builder.Services.AddApiVersioning(opts =>
            {
                opts.AssumeDefaultVersionWhenUnspecified = true;
                opts.DefaultApiVersion = new ApiVersion(2, 0);
                opts.ReportApiVersions = true;
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        }

        public static void AddCustomServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IDataAccess, SqlDataAccess>();
            builder.Services.AddScoped<ITodoDataService, TodoDataService>();
        }

        public static void AddSecurity(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization(opts =>
            {
                opts.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            string? secretKey = builder.Configuration.GetValue<string>("Authentication:SecretKey");
            if (secretKey is null)
            {
                throw new Exception("Secret key is missing");
            }
            else
            {
                builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer(opts =>
                {
                    opts.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration.GetValue<string>("Authentication:Issuer"),
                        ValidAudience = builder.Configuration.GetValue<string>("Authentication:Audience"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
                    };
                });
            }
        }

        public static void AddHealth(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("Default");
            if (connectionString is null)
            {
                throw new Exception("Connection string is missing");
            }
            else
            {
                builder.Services.AddHealthChecks()
                    .AddSqlServer(connectionString);
            }
        }
    }
}
