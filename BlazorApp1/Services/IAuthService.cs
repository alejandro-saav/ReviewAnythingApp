using BlazorApp1.Models.Auth;

namespace BlazorApp1.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterRequestDto request);
    Task<bool> ConfirmEmailAsync(string userId, string token);
    
    string? LastErrorMessage { get; }
}