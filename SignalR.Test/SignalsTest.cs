namespace MySignalR.Test;

using System.Net;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

public class SignalsTest: IClassFixture<TestWebApplicationFactory<TestStartup>>
{
    protected HttpClient Client;
    protected readonly ITestOutputHelper Output;
    protected const string TestEndpoint = "/api/test";
    protected TestWebApplicationFactory<TestStartup> factory;
    
    public SignalsTest(TestWebApplicationFactory<TestStartup> factory, ITestOutputHelper output)
    {
        // Define Output helper
        this.Output = output;

        this.factory = factory;
        //this.SetupSignalHubClient(factory);
        // Define client
        this.Client = factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddMvc().AddApplicationPart(typeof(Startup).Assembly);
                });
            })
            .CreateClient(); // Create HttpClient
    }

    [Fact]
    public async void DirectContextCallTest()
    {
        var connection = new HubConnectionBuilder()
            .WithUrl($"{factory.Server.BaseAddress}signalr-hub", o => o.HttpMessageHandlerFactory = _ => factory.Server.CreateHandler())
            .Build();

        var tcs = new TaskCompletionSource<string>();

        connection.On<string>(
            "TestSignal",
            message => {
                tcs.TrySetResult(message); 
            });

        await connection.StartAsync();

        var signalHub = this.factory.Services.GetService<IHubContext<SignalHub, ISignal>>();
        await signalHub.Clients.All.TestSignal("Test String");

        string result = null;
        CancellationTokenSource cts = new CancellationTokenSource(5000);
        cts.Token.Register(() => tcs.TrySetCanceled());
        try
        {
            result = await tcs.Task;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\nTasks cancelled: timed out.\n");
        }
        finally
        {
            cts.Dispose();
        }

        Assert.Equal("Test String", result);
    }

    [Fact]
    public async void ApiTest()
    {
        var connection = new HubConnectionBuilder()
            .WithUrl($"{factory.Server.BaseAddress}signalr-hub", o => o.HttpMessageHandlerFactory = _ => factory.Server.CreateHandler())
            .Build();

        var tcs = new TaskCompletionSource<string>();

        connection.On<string>(
            "TestSignal",
            message => {
                tcs.TrySetResult(message); 
            });

        await connection.StartAsync();

        // Act
        var responsePost = await this.Client.PostAsync(TestEndpoint, null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, responsePost.StatusCode);
        
        string result = null;
        CancellationTokenSource cts = new CancellationTokenSource(5000);
        cts.Token.Register(() => tcs.TrySetCanceled());
        try
        {
            result = await tcs.Task;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\nTasks cancelled: timed out.\n");
        }
        finally
        {
            cts.Dispose();
        }

        Assert.Equal("Test String", result);
    }
}