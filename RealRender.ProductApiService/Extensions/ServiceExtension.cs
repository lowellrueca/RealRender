using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RealRender.ProductApiService.Db;
using RealRender.ProductApiService.Profiles;
using RealRender.ProductApiService.Repositories;
using System.Reflection;
namespace RealRender.ProductApiService.Extensions;

public static class ServiceExtension
{
    private const string _oauth = "oauth2";
    private const string _scope = "product-api-service";

    public static void ConfigureApplicationDbContext(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>((serviceProvider, optionsBuilder) =>
        {
            var connectionString = serviceProvider.GetRequiredService<IConfiguration>()["productdb"];
            var migrationAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
            optionsBuilder.UseNpgsql(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(migrationAssembly);
            });
        });
    }

    public static void AddRepositoryManager(this IServiceCollection service)
    {
        service.AddScoped<IRepositoryManager, RepositoryManager>();
    }

    public static void ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(swaggerGenOptions =>
        {
            swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo { Title = "Product Api", Version = "v1" });

            swaggerGenOptions.AddSecurityDefinition(_oauth,
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri($"{configuration["Authentication:Authority"]}/connect/token"),
                            Scopes = { { _scope, "Product Api" } }
                        }
                    }
                });

            swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Id = _oauth, Type = ReferenceType.SecurityScheme }
                    },
                    new string[] { _scope }
                },
            });
        });
    }

    public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(authenticationOptions =>
        {
            authenticationOptions.RequireAuthenticatedSignIn = true;
            authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer("Bearer", jwtBearerOptions =>
        {
            jwtBearerOptions.MetadataAddress = $"{configuration["Authentication:Authority"]}/.well-known/openid-configuration";
            jwtBearerOptions.RequireHttpsMetadata = false;
            jwtBearerOptions.SaveToken = true;
            jwtBearerOptions.Authority = configuration["Authentication:Authority"];
            jwtBearerOptions.Audience = configuration["Authentication:Audience"];
            jwtBearerOptions.BackchannelHttpHandler = GetHttpClientHandler();
            jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidTypes = new[] { "at+jwt" }
            };
        });
    }

    public static void ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(authorizationOptions =>
        {
            authorizationOptions.AddPolicy("ApiScope", authorizationPolicyBuilder =>
            {
                authorizationPolicyBuilder.RequireAuthenticatedUser()
                    .RequireClaim("scope", _scope).Build();
            });
        });
    }

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(corsOptions =>
        {
            corsOptions.AddDefaultPolicy(corsPolicyBuilder =>
            {
                corsPolicyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });
    }

    public static void ConfigureProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ProductProfile));
    }

    private static HttpClientHandler GetHttpClientHandler()
    {
        var handler = new HttpClientHandler();
        handler.ClientCertificateOptions = ClientCertificateOption.Manual;
        handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
        {
            return true;
        };

        return handler;
    }
}
