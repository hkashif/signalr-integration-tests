namespace MySignalR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly IHubContext<SignalHub, ISignal> hub;

    public TestController(IHubContext<SignalHub, ISignal> hub)
    {
        this.hub = hub;
    }

    [HttpPost()]
    public async Task<IActionResult> Post()
    {
        await this.hub.Clients.All.TestSignal("Test String");
        return Ok();
    }
}
