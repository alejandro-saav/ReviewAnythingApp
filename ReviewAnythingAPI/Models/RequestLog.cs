namespace ReviewAnythingAPI.Models;

public class RequestLog
{
    public int Id {get;set;}
    public string IpAddress {get;set;} = "Unknown";
    public string UserAgent {get;set;} = "Unknown";
    public string AcceptLanguage {get;set;} = "Unknown";
    public DateTime CreatedAt {get;set;} = DateTime.UtcNow;
}