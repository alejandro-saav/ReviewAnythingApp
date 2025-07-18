namespace BlazorApp1.Models.Auth;

public class GoogleAuthResult
{
    public bool Success { get; set; }
    public string Error { get; set; }
    public string CustomJwt { get; set; }
}