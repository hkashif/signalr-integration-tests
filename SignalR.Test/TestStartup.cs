namespace MySignalR.Test;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MySignalR;

public class TestStartup : Startup
{
    public TestStartup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
    {
    }
}