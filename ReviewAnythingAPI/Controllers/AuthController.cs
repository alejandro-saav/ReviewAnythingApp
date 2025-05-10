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
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto userRegistrationDto)
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
}