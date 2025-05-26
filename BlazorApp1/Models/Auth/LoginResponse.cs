namespace BlazorApp1.Models.Auth;

public class LoginResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Token { get; set; }
    public User? UserResponse { get; set; }
    public List<string>? Errors { get; set; }
}