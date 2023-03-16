namespace MySignalR.Test;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override IWebHostBuilder CreateWebHostBuilder()
    {
        return WebHost.CreateDefaultBuilder().UseStartup<TStartup>();
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile("appsettings.Test.json");
        });
        base.ConfigureWebHost(builder);
    }
}