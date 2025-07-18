using BlazorApp1.Models.Auth;

namespace BlazorApp1.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterRequestDto request);
    Task<bool> ConfirmEmailAsync(string userId, string token);
    Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<bool> ResetPasswordAsync(string userId, string token, string newPassword);
    Task<LoginResponse> GoogleLoginAsync(string idToken);
    
    string? LastErrorMessage { get; }
}