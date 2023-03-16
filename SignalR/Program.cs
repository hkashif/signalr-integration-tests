namespace MySignalR;

using Serilog;

public class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .CreateBootstrapLogger();
        try 
        {
            Log.Information("Starting web host");
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }
        catch (Exception ex) 
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally 
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services))
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}