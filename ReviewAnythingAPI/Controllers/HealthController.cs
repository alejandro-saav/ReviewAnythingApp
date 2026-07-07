using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ReviewAnythingAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[Controller]")]
public class HealthController : ControllerBase
{
    [HttpHead]
    public IActionResult WakeUp()
    {
        return Ok();
    }
}