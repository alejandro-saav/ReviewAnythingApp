using Microsoft.AspNetCore.Identity.Data;
using ReviewAnythingAPI.DTOs.AuthDTOs;
using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterUserAsync(UserRegistrationRequestDto userRegistrationDto);
    
    Task<AuthResponseDto> LoginUserAsync(UserLoginRequestDto userLoginDto);

    Task<string> GenerateJwtToken(ApplicationUser user);
    Task<AuthResponseDto> GoogleSignInAsync(string idToken);
    Task<AuthResponseDto> ConfirmEmailAsync(string userId, string token);
    Task<AuthResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequestDto);
    Task<AuthResponseDto> ResetPasswordAsync(string userId, string token, ResetPasswordRequestDto resetPasswordRequestDto);
}