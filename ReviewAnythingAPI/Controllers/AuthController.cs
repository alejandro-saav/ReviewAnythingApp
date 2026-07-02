using System.Net;
using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReviewAnythingAPI.DTOs.AuthDTOs;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v(version:apiVersion)/[Controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user and sends an account confirmation email.
    /// </summary>
    /// <remarks>Password must contain at least 8 characters, one uppercase letter, one number and one symbol.</remarks>
    /// <returns>A success boolean, a message and the user information.</returns>
    /// <response code="200">Successful registration. returns a success response.</response>
    /// <response code="400">Invalid credentials or missing fields. Check email format, password constrainst and all the required fields.</response>
    /// <response code="409">User already exists with the provided email adress or username.</response>
    /// <response code="500">Internal server error. Please try again.</response>

    [HttpPost("Register")]
    [ProducesResponseType(typeof(SuccessAuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromForm] UserRegistrationRequestDto userRegistrationDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.RegisterUserAsync(userRegistrationDto);

        _logger.LogInformation("New user registered. User id: {UserId}, username: {UserName}, at: {Time}", result.UserResponse.UserId, result.UserResponse.UserName, DateTime.UtcNow);
        return Ok(result);
    }


    /// <summary>
    /// Authenticates a user and returns a JWT upon successful login.
    /// </summary>
    /// <remarks>Password must contain at least 8 characters, one uppercase letter, one number and one symbol.</remarks>
    /// <returns>A JWT token, a success boolean, a message and the user information.</returns>
    /// <response code="200">Successful login. returns a success response.</response>
    /// <response code="400">Invalid credentials or missing fields. Check email format and password constrainst.</response>
    /// <response code="409">User does not exists, has not confirmed email yet or has been locked out.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpPost("Login")]
    [ProducesResponseType(typeof(SuccessAuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto userLoginDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _authService.LoginUserAsync(userLoginDto);

        _logger.LogInformation("New login. User id: {UserId}, username: {UserName}, at: {Time}", result.UserResponse.UserId, result.UserResponse.UserName, DateTime.UtcNow);

        var reactHeader = Request.Headers["X-Client-Type"].FirstOrDefault();
        if (reactHeader is not null)
        {
            Response.Cookies.Append("accessToken", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromDays(7)
            });
            var webResponse = new ReactLoginResponseDto
            {
                ProfileImage = result.UserResponse.ProfileImage,
                FirstName = result.UserResponse.FirstName,
                LastName = result.UserResponse.LastName,
                Bio = result.UserResponse.Bio
            };
            return Ok(webResponse);
        }

        return Ok(result);
    }

    /// <summary>
    /// Authenticates a user through google auth and returns a JWT upon successful login.
    /// </summary>
    /// <returns>A JWT token, a success boolean, a message and the user information.</returns>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpPost("google-signin")]
    [ProducesResponseType(typeof(SuccessAuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInRequestDto googleSignInDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var authResponse = await _authService.GoogleSignInAsync(googleSignInDto.IdToken);

        _logger.LogInformation("New google sign in. User id: {UserId}, username: {UserName}, at: {Time}", authResponse.UserResponse.UserId, authResponse.UserResponse.UserName, DateTime.UtcNow);

        var reactHeader = Request.Headers["X-Client-Type"].FirstOrDefault();
        if (reactHeader is not null)
        {
            Response.Cookies.Append("accessToken", authResponse.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromDays(7)
            });
            var webResponse = new ReactLoginResponseDto
            {
                ProfileImage = authResponse.UserResponse.ProfileImage,
                FirstName = authResponse.UserResponse.FirstName,
                LastName = authResponse.UserResponse.LastName,
                Bio = authResponse.UserResponse.Bio
            };
            return Ok(webResponse);
        }
        return Ok(authResponse);
    }

    /// <summary>
    /// Confirms the user's email and returns a JWT.
    /// </summary>
    /// <param name="userId">The unique identifier (string) of the user to confirm.</param>
    /// <param name="token">The email confirmation token generated by Identity, typically extracted from the verification link sent to the user.</param>
    /// <returns>A JWT token, a success boolean, and the user information.</returns>
    /// <response code="200">Email confirmation successful. returns a success response.</response>
    /// <response code="400">Returned if the userId or token is null or empty.</response>
    /// <response code="404">User does not exists or the token has expired for this user.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpGet("confirm-email")]
    [ProducesResponseType(typeof(SuccessAuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(SuccessAuthResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return BadRequest("userId or token is empty");
        var result = await _authService.ConfirmEmailAsync(userId, token);

        _logger.LogInformation("Email successfully confirmed. User id: {UserId}, username: {UserName}, at: {Time}", result.UserResponse.UserId, result.UserResponse.UserName, DateTime.UtcNow);
        return Ok(result);
    }

    /// <summary>
    /// Sends an email for reseting the user's password.
    /// </summary>
    /// <returns>A success boolean, a message and the user information.</returns>
    /// <response code="200">Email send successful. returns a success response.</response>
    /// <response code="409">User does not exists or has not confirmed email yet.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(SuccessAuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);

        _logger.LogInformation("Email for password reset successfully sent. User id: {UserId}, username: {UserName}, at: {Time}", result.UserResponse.UserId, result.UserResponse.UserName, DateTime.UtcNow);

        return Ok(new GenericResponseDto
        {
            Success = result.Success,
            Message = result.Message
        });
    }


    /// <summary>
    /// Reset the user's password.
    /// </summary>
    /// <returns>A success boolean, a message and the user information.</returns>
    /// <response code="200">Password reset successful. returns a success response.</response>
    /// <response code="409">User does not exists or has not confirmed email yet.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(SuccessAuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Log the user out by removing the JWT from the header request.
    /// </summary>
    /// <returns>A success boolean, a message and the user information.</returns>
    /// <response code="200">Password reset successful. returns a success response.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(SuccessAuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LogoutBrowser()
    {
        Response.Cookies.Delete("accessToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax
        });

        return Ok();
    }

    /// <summary>
    /// Deletes the user account.
    /// </summary>
    /// <returns>A success boolean.</returns>
    /// <response code="200">Account deletion successful. returns a success response.</response>
    /// <response code="403">Unauthorized to perform the delete operation.</response>
    /// <response code="409">User does not exist.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [Authorize]
    [HttpDelete("delete-account")]
    [ProducesResponseType(typeof(SuccessAuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAccount()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !int.TryParse(userIdClaim, out int userId)) throw new UnauthorizedAccessException("UserId claim not found or unable to convert the userId to int");
        var deleteAccount = await _authService.DeleteAccountAsync(userIdClaim);

        _logger.LogInformation("Account successfully deleted for userId: {0}", userIdClaim);
        return NoContent();
    }
}