using Microsoft.EntityFrameworkCore;
using RealRender.ProductApiService.Db;
using Serilog;
namespace RealRender.ProductApiService.Extensions;

public static class ApplicationExtension
{
    public static void InitializeMigration(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            bool migrated = false;
            var serviceProvider = scope.ServiceProvider;
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            var connected = context.Database.CanConnect();

            Log.Debug($"App is connected to db: {connected}");

            if (context.Database.GetAppliedMigrations().Any())
            {
                migrated = true;
                Log.Debug("Migration has been initialized.");
            }

            if (context.Database.GetPendingMigrations().Any())
            {
                Log.Debug("Attempting to migrate...");
                context.Database.Migrate();
                migrated = true;
                Log.Debug("Migration successful");
            }

            if (!migrated)
            {
                Log.Debug("Migration unsuccesful.");
            }
        }
    }
}
