using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ReviewAnythingAPI.DTOs.LogDTOs;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[Controller]")]
public class LogController : ControllerBase
{
    private readonly ILogger<LogController> _logger;
    private readonly ILogService _logService;

    public LogController(ILogger<LogController> logger, ILogService logService)
    {
        _logger = logger;
        _logService = logService;
    }


    /// <summary>
    /// Create a new log resource with the request headers information.
    /// </summary>
    /// <returns>The newly created log with details including: Id, ip address, user agent, accept language and created at.</returns>
    /// <response code="201">Log successfully created, returns the newly created log.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpPost]
    [ProducesResponseType(typeof(RequestLog), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(RequestLog), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> InsertNewLogVisitAsync()
    {
        LogInsertRequestDto newLogDto = new LogInsertRequestDto
        {
            UserAgent = Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown",
            AcceptLanguage = Request.Headers["Accept-Language"].FirstOrDefault() ?? "Unknown",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "",
            CreatedAt = DateTime.UtcNow,
        };
        var response = await _logService.InsertNewVisitLogAsync(newLogDto);

        _logger.LogInformation("New visit log has been inserted. Log ID: {LogId}, at: {Time}", response.Id, DateTime.UtcNow.AddHours(-5));

        return CreatedAtAction(nameof(GetLogById), new { logId = response.Id }, response);
    }


    /// <summary>
    /// Retrieves the details of a log by its unique identifier.
    /// </summary>
    /// <param name="logId">The unique identifier of a log. (int)</param>
    /// <returns>A log with details including: Id, ip address, user agent, browser language and creation date.</returns>
    /// <response code="200">Log found returns the log details.</response>
    /// <response code="404">A log was not found for the given log id.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpGet("{logId:int}")]
    [ProducesResponseType(typeof(RequestLog), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RequestLog), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(RequestLog), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLogById([FromRoute] int logId)
    {
        var response = await _logService.GetVisitLogByIdAsync(logId);

        _logger.LogInformation("Log retrieved successfully. Log ID: {LogId}, at: {Time}", response.Id, DateTime.UtcNow.AddHours(-5));

        return Ok(response);
    }

    /// <summary>
    /// Retrieves a list of all logs.
    /// </summary>
    /// <returns>A list of all logs.</returns>
    /// <response code="200">A list of all logs.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [Authorize]
    [HttpGet("all")]
    [ProducesResponseType(typeof(RequestLog), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RequestLog), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllLogsAsync()
    {
        var response = await _logService.GetAllLogsAsync();

        _logger.LogInformation("All logs retrieved successfully. Logs count: {LogsCount}, at: {Time}", response.Count(), DateTime.UtcNow.AddHours(-5));

        return Ok(response);
    }

}