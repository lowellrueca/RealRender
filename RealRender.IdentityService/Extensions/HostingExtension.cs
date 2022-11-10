using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealRender.IdentityService.Db;
using RealRender.IdentityService.Models;
using Serilog;
namespace RealRender.IdentityService.Extensions;

public static class HostingExtension
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.ConfigureLogging();
        builder.Services.ConfigureApplicationDbContext();
        builder.Services.ConfigureIdentity();
        builder.Services.ConfigureIdentityServer();
        builder.Services.AddAuthorization();
        return builder.Build();
    }

    public static void ConfigurePipeline(this WebApplication application)
    {
        application.UseSerilogRequestLogging();
        application.UseHttpsRedirection();
        application.UseIdentityServer();
        application.UseAuthorization();

        InitializeDb(application);

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

    private static async void InitializeDb(IApplicationBuilder application)
    {
        using (var serviceScope = application.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        {
            serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
            serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
            serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
            
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            if (await userManager.FindByNameAsync("lowellrueca") == null)
            {
                var testUser = new ApplicationUser { UserName = "lowellrueca", Email = "lowellrueca@gmail.com" };
                await userManager.CreateAsync(testUser, "default");
            }

            var configurationDbContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            if (!await configurationDbContext.Clients.AnyAsync())
            {
                foreach (var client in Config.Clients)
                {
                    await configurationDbContext.Clients.AddAsync(client.ToEntity());
                }
                await configurationDbContext.SaveChangesAsync();
            }
            if (!await configurationDbContext.IdentityResources.AnyAsync())
            {
                foreach (var resource in Config.IdentityResources)
                {
                    await configurationDbContext.IdentityResources.AddAsync(resource.ToEntity());
                }
                await configurationDbContext.SaveChangesAsync();
            }

            if (!await configurationDbContext.ApiScopes.AnyAsync())
            {
                foreach (var resource in Config.ApiScopes)
                {
                    await configurationDbContext.ApiScopes.AddAsync(resource.ToEntity());
                }
                await configurationDbContext.SaveChangesAsync();
            }
        }
    }
}
