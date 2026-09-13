using AutoOps.Api.Services;
using AutoOps.Contracts.Events;
using Microsoft.AspNetCore.Mvc;

namespace AutoOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HeartbeatController : ControllerBase
{
    private readonly ServiceBusPublisher _serviceBusPublisher;

    public HeartbeatController(ServiceBusPublisher serviceBusPublisher)
    {
        _serviceBusPublisher = serviceBusPublisher;
    }

    [HttpPost]
    public async Task<IActionResult> RequestHeartbeat(string target)
    {
        HeartbeatRequested heartbeatRequested = new (
            Guid.NewGuid(),
            target,
            DateTime.UtcNow);

        await _serviceBusPublisher.PublishAsync(heartbeatRequested);

        return Accepted(heartbeatRequested);
    }
}