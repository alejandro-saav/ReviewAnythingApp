using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ReviewAnythingAPI.Controllers;

[ApiController]
[Route("api/[Controller]")]
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