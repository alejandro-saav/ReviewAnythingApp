namespace BlazorApp1.HelperClasses;

public class ClientInfoService
{
    public string IpAddress { get; set; } = "Unknown";
    public string UserAgent { get; set; } = "Unknown";
    public string AcceptLanguage { get; set; } = "Unknown";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}