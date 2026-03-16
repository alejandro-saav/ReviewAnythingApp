using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ReviewAnythingAPI.DTOs.LogDTOs;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;

[ApiController]
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

    [HttpPost]
    public async Task<IActionResult> InsertNewLogVisitAsync([FromBody] LogInsertRequestDto logRequest)
    {
        if (!ModelState.IsValid) throw new ArgumentException("Invalid or missing required fields.");
        var response = await _logService.InsertNewVisitLogAsync(logRequest);

        _logger.LogInformation("New visit log has been inserted. Log ID: {LogId}, at: {Time}", response.Id, DateTime.UtcNow.AddHours(-5));

        return Ok(response);
    }

    [HttpGet("{logId:int}")]
    public async Task<IActionResult> GetLogByIdAsync([FromRoute] int logId)
    {
        var response = await _logService.GetVisitLogByIdAsync(logId);

        _logger.LogInformation("Log retrieved successfully. Log ID: {LogId}, at: {Time}", response.Id, DateTime.UtcNow.AddHours(-5));

        return Ok(response);
    }

    [Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllLogsAsync()
    {
        var response = await _logService.GetAllLogsAsync();

        _logger.LogInformation("All logs retrieved successfully. Logs count: {LogsCount}, at: {Time}", response.Count(), DateTime.UtcNow.AddHours(-5));

        return Ok(response);
    }

}