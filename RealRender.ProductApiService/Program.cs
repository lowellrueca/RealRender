using Serilog;
using RealRender.ProductApiService.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting the application...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.ConfigureServices()
           .ConfigurePipeline();
}

catch (Exception exc)
{
    Log.Fatal(exc, "Unhandled exception");
}

finally
{
    Log.Information("Shut down complete.");
    Log.CloseAndFlush();
}
