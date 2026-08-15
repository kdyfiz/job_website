using Microsoft.AspNetCore.Mvc;

namespace JobScout.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            service = "JobScout.API",
            timestamp = DateTimeOffset.UtcNow
        });
    }
}
