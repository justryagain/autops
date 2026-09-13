using AutoOps.Contracts.Events;
using Microsoft.AspNetCore.Mvc;

namespace AutoOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HeartbeatController : ControllerBase
{
    [HttpPost]
    public IActionResult RequestHeartbeat(string target)
    {
        HeartbeatRequested heartbeatRequested = new (
            Guid.NewGuid(),
            target,
            DateTime.UtcNow);

        return Accepted(heartbeatRequested);
    }
}