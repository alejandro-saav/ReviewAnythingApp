using Microsoft.AspNetCore.Mvc;
using ReviewAnythingAPI.DTOs.AuthDTOs;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromForm] UserRegistrationRequestDto userRegistrationDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var result = await _authService.RegisterUserAsync(userRegistrationDto);
        
        if (!result.Success) return BadRequest(result.Errors);

        return Ok(result);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto userLoginDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.LoginUserAsync(userLoginDto);
        
        if (!result.Success) return BadRequest(result.ErrorMessage);
        return Ok(result);
    }

    [HttpPost("google-signin")]
    public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInRequestDto googleSignInDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var authResponse = await _authService.GoogleSignInAsync(googleSignInDto.IdToken);
        if (authResponse.Success)
        {
            return Ok(authResponse);
        }
        return BadRequest(new { authResponse.ErrorMessage, authResponse.Errors });
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return BadRequest("userId or token is empty");
        var result = await _authService.ConfirmEmailAsync(userId, token);
        return Ok(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromQuery] string userId, [FromQuery] string token, [FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.ResetPasswordAsync(userId, token, resetPasswordRequestDto);
        if (result.Success) return Ok(result);
        return BadRequest(result);
    }
}