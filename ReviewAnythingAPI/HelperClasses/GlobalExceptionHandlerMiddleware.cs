using System.Net;
using System.Text.Json;
using ReviewAnythingAPI.HelperClasses.CustomExceptions;

namespace ReviewAnythingAPI.HelperClasses;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, _logger);
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
        }
        logger.LogError(exception, "Exception caught in global handler");

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