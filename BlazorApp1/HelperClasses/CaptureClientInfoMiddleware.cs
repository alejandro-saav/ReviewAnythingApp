using System.Text.Json.Serialization;
using BlazorApp1.Models.Auth;
using BlazorApp1.Services;

namespace BlazorApp1.HelperClasses;

public class CaptureClientInfoMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CaptureClientInfoMiddleware> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public CaptureClientInfoMiddleware(RequestDelegate next, ILogger<CaptureClientInfoMiddleware> logger, IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task InvokeAsync(HttpContext context, ClientInfoService clientInfo)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            bool isAPIUp = await userService.WakeAPI();
            if (isAPIUp)
            {
                _logger.LogInformation("Api is up. 👍");
            } else
            {
                _logger.LogInformation("Api is down. 👎");
            }
            
            if (!context.Request.Cookies.ContainsKey("visit_tracked"))
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = context.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";
                var acceptLanguage = context.Request.Headers["Accept-Language"].FirstOrDefault() ?? "Unknown";
                clientInfo.IpAddress = ipAddress;
                clientInfo.AcceptLanguage = acceptLanguage;
                clientInfo.UserAgent = userAgent;
                clientInfo.CreatedAt = DateTime.UtcNow.AddHours(-5);
                _logger.LogInformation("New visit by {IpAddress} at {Time}", ipAddress, DateTime.UtcNow.AddHours(-5));


                bool userLogResponse = await userService.LogVisitAsync(clientInfo);
                if (userLogResponse)
                {
                    context.Response.Cookies.Append("visit_tracked", "1", new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Lax,
                        MaxAge = TimeSpan.FromMinutes(30)
                    });
                }
            }
        }
        await _next(context);
    }
}