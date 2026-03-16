using Microsoft.AspNetCore.Identity.Data;
using ReviewAnythingAPI.DTOs.AuthDTOs;
using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Services.Interfaces;

public interface IAuthService
{
    Task<SuccessAuthResponseDto> RegisterUserAsync(UserRegistrationRequestDto userRegistrationDto);
    
    Task<SuccessAuthResponseDto> LoginUserAsync(UserLoginRequestDto userLoginDto);

    Task<string> GenerateJwtToken(ApplicationUser user);
    Task<SuccessAuthResponseDto> GoogleSignInAsync(string idToken);
    Task<SuccessAuthResponseDto> ConfirmEmailAsync(string userId, string token);
    Task<SuccessAuthResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequestDto);
    Task<SuccessAuthResponseDto> ResetPasswordAsync(string userId, string token, ResetPasswordRequestDto resetPasswordRequestDto);
}