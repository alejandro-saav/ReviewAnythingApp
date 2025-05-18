using ReviewAnythingAPI.DTOs.AuthDTOs;
using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterUserAsync(UserRegistrationRequestDto userRegistrationDto);
    
    Task<AuthResponseDto> LoginUserAsync(UserLoginRequestDto userLoginDto);

    Task<string> GenerateJwtToken(ApplicationUser user);
    Task<AuthResponseDto> GoogleSignInAsync(string idToken);
}