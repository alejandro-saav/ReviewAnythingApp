using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ReviewAnythingAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[Controller]")]
public class HealthController : ControllerBase
{
    [HttpGet("visit")]
    public IActionResult TrackVisit()
    {
        return Ok();
    }

    [HttpHead]
    public IActionResult WakeUp()
    {
        return Ok();
    }
}