namespace MySignalR;

using Microsoft.AspNetCore.SignalR;
using Serilog;

public class SignalHub : Hub<ISignal>
{
    public Task IncomingSignal(string message)
    {
        Log.Information($"Incoming Signal -- Message: '{message}'");
        return Task.CompletedTask;
    }

    public override async Task OnConnectedAsync()
    {
        Log.Information("SignalR: Client Connected");
        await base.OnConnectedAsync();
    }
    public override async Task OnDisconnectedAsync(Exception e)
    {
        Log.Information("SignalR: Client Disconnected");
        await base.OnDisconnectedAsync(e);
    }
}