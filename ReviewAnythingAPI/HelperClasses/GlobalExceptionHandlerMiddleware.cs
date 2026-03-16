using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.HelperClasses.CustomExceptions;

namespace ReviewAnythingAPI.HelperClasses;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();

        context.Items["CorrelationId"] = correlationId;
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["Path"] = context.Request.Path,
            ["Method"] = context.Request.Method
        }))
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();

                _logger.LogInformation("➡️ Request started: {Method} {Path}", context.Request.Method, context.Request.Path);
                await _next(context);

                stopwatch.Stop();

                _logger.LogInformation("⬅️ Request completed: {Method} {Path} - {StatusCode} in {ElapsedMs}ms", context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError("💥 Request failed: {Method} {Path} - Error: {Message}", context.Request.Method, context.Request.Path, ex.Message);

                await HandleExceptionAsync(context, ex, _logger);
            }
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
    {
        // Default values
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred";

        // customize based on exception type
        switch (exception)
        {
            case ArgumentException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Forbidden;
                message = "You are not authorized to perform this action.";
                break;
            case EntityNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
                break;
            case InvalidOperationException:
                statusCode = HttpStatusCode.Conflict;
                message = exception.Message;
                break;
            case TransactionFailedException:
                statusCode = HttpStatusCode.InternalServerError;
                message = exception.Message;
                break;
            case Exception:
                statusCode = HttpStatusCode.InternalServerError;
                message = exception.Message;
                break;
        }

        var response = new
        {
            error = message,
            statusCode = (int)statusCode
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}