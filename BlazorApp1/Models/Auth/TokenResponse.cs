namespace BlazorApp1.Models.Auth;

public class TokenResponse
{
    public string AccessToken { get; set; }
    public string IdToken { get; set; }
    public string RefreshToken { get; set; }
    public string TokenType { get; set; }
    public int ExpiresIn { get; set; }
}