using Microsoft.IdentityModel.Logging;
using RealRender.ProductApiService.Filters;
using Serilog;
using System.Net;
namespace RealRender.ProductApiService.Extensions;

public static class HostingExtension
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        ServicePointManager.Expect100Continue = true;
        IdentityModelEventSource.ShowPII = true;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

        builder.ConfigureLogging();

        builder.Services.AddControllers();

        builder.Services.ConfigureJwtAuthentication(configuration);
        builder.Services.ConfigureAuthorization();
        
        builder.Services.ConfigureCors();

        builder.Services.ConfigureApplicationDbContext();

        builder.Services.AddRepositoryManager();

        builder.Services.AddScoped<AnyDbContentFilter>();

        builder.Services.ConfigureProfiles();

        builder.Services.ConfigureSwagger(configuration);
        
        return builder.Build();
    }

    public static void ConfigurePipeline(this WebApplication application)
    {
        application.UseSerilogRequestLogging();

        application.UseSwagger();
        application.UseSwaggerUI();

        application.InitializeMigration();

        application.UseHttpsRedirection();

        application.UseCors();

        application.UseAuthentication();

        application.UseAuthorization();

        application.MapControllers().RequireAuthorization("ApiScope");
        
        application.Run();
    }

    private static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Host.UseSerilog((context, logger) => logger
        .Enrich.FromLogContext()
            .ReadFrom.Configuration(context.Configuration)
        );
    }
}
