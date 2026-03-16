using Microsoft.AspNetCore.Mvc;
using ReviewAnythingAPI.DTOs.AuthDTOs;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromForm] UserRegistrationRequestDto userRegistrationDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.RegisterUserAsync(userRegistrationDto);

        _logger.LogInformation("New user registered. User id: {UserId}, username: {UserName}, at: {Time}", result.UserResponse.UserId, result.UserResponse.UserName, DateTime.UtcNow);
        return Ok(result);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto userLoginDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.LoginUserAsync(userLoginDto);

        _logger.LogInformation("New login. User id: {UserId}, username: {UserName}, at: {Time}", result.UserResponse.UserId, result.UserResponse.UserName, DateTime.UtcNow);

        return Ok(result);
    }

    [HttpPost("google-signin")]
    public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInRequestDto googleSignInDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var authResponse = await _authService.GoogleSignInAsync(googleSignInDto.IdToken);

        _logger.LogInformation("New google sign in. User id: {UserId}, username: {UserName}, at: {Time}", authResponse.UserResponse.UserId, authResponse.UserResponse.UserName, DateTime.UtcNow);
        return Ok(authResponse);
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return BadRequest("userId or token is empty");
        var result = await _authService.ConfirmEmailAsync(userId, token);

        _logger.LogInformation("Email successfully confirmed. User id: {UserId}, username: {UserName}, at: {Time}", result.UserResponse.UserId, result.UserResponse.UserName, DateTime.UtcNow);
        return Ok(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);

        _logger.LogInformation("Email for password reset successfully sent. User id: {UserId}, username: {UserName}, at: {Time}",result.UserResponse.UserId, result.UserResponse.UserName, DateTime.UtcNow);

        return Ok(new GenericResponseDto
        {
            Success = result.Success,
            Message = result.Message
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromQuery] string userId, [FromQuery] string token, [FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.ResetPasswordAsync(userId, token, resetPasswordRequestDto);

        _logger.LogInformation("Password successfully reset. User id: {UserId}, username: {UserName}, at: {Time}", result.UserResponse.UserId, result.UserResponse.UserName, DateTime.UtcNow);

        return Ok(new GenericResponseDto
        {
            Success = result.Success,
            Message = result.Message
        });
    }
}