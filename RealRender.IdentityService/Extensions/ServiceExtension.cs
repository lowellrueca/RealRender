using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using RealRender.IdentityService.Db;
using RealRender.IdentityService.Models;
using System.Reflection;
namespace RealRender.IdentityService.Extensions;

public static class ServiceExtension
{
    public static void ConfigureApplicationDbContext(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(IdentityDbContextOptions);
    }

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
        })
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationDbContext>();
    }

    public static void ConfigureIdentityServer(this IServiceCollection services)
    {
        services.AddIdentityServer()
            .AddConfigurationStore(configurationStoreOptions =>
            {
                configurationStoreOptions.ResolveDbContextOptions = IdentityServerDbContextOptions;
            })
            .AddOperationalStore(operationalStoreOptions =>
            {
                operationalStoreOptions.ResolveDbContextOptions = IdentityServerDbContextOptions;
            })
            .AddAspNetIdentity<ApplicationUser>();
    }

    private static void IdentityDbContextOptions(IServiceProvider serviceProvider, DbContextOptionsBuilder dbContextOptions)
    {
        var connectionString = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("identitydb");
        dbContextOptions.UseNpgsql(connectionString, SqlOptions);
    }

    private static void IdentityServerDbContextOptions(IServiceProvider serviceProvider, DbContextOptionsBuilder dbContextOptionsBuilder)
    {
        var connectionString = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("identityserverdb");
        dbContextOptionsBuilder.UseNpgsql(connectionString, SqlOptions);
    }

    private static void SqlOptions(NpgsqlDbContextOptionsBuilder dbContextOptionsBuilder)
    {
        dbContextOptionsBuilder.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
    }
}
