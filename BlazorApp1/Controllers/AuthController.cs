using System.Security.Claims;
using BlazorApp1.Models.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly HttpClient _apiHttpClient; // This HttpClient calls your actual .NET 9 API
    private readonly IConfiguration _configuration;

    public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _apiHttpClient = httpClientFactory.CreateClient("ReviewAnythingAPI");;
        _configuration = configuration;
    }
    
    [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        { 
            var apiResponse = await _apiHttpClient.PostAsJsonAsync("api/auth/Login", request);

            if (apiResponse.IsSuccessStatusCode)
            {
                var loginApiResponse = await apiResponse.Content.ReadFromJsonAsync<LoginResponse>(); 

                if (loginApiResponse == null || string.IsNullOrEmpty(loginApiResponse.Token))
                {
                    return BadRequest("Authentication failed: No token received.");
                }
                // var claims = new List<Claim>
                // {
                //     new Claim(ClaimTypes.Name, loginApiResponse.UserResponse.FirstName),
                //     new Claim("AccessToken", loginApiResponse.Token)
                // };
                //
                // var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                // var principal = new ClaimsPrincipal(identity);
                // await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return Ok(loginApiResponse);
            }
            else
            {
                // 5. Proxy the error response back to the Blazor client
                var errorContent = await apiResponse.Content.ReadAsStringAsync();
                return StatusCode((int)apiResponse.StatusCode, errorContent);
            }
        }
        
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
            var apiResponse = await _apiHttpClient.PostAsJsonAsync("api/auth/Register", request);
            if (apiResponse.IsSuccessStatusCode)
            {
                return Ok(new { Message = "Registration successful. We have sent a confirmation email to the register address." });
            }
            else
            {
                var errorContent = await apiResponse.Content.ReadAsStringAsync();
                return StatusCode((int)apiResponse.StatusCode, errorContent);
            }
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return BadRequest("userId or token is empty");
        try
        {
            var decodedToken = Uri.UnescapeDataString(token);
            var apiResponse = await _apiHttpClient.GetAsync($"api/auth/confirm-email?userId={userId}&token={decodedToken}");

            if (apiResponse.IsSuccessStatusCode)
            {
                var loginApiResponse = await apiResponse.Content.ReadFromJsonAsync<LoginResponse>();

                if (loginApiResponse == null || string.IsNullOrEmpty(loginApiResponse.Token))
                {
                    return BadRequest("Authentication failed: No token received.");
                }

                Response.Cookies.Append("AuthToken", loginApiResponse.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30)
                });
                return Ok(new { Message = "Login successful" });
            }
            else
            {
                var errorContent = await apiResponse.Content.ReadAsStringAsync();
                return StatusCode((int)apiResponse.StatusCode, errorContent);
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error occurred during email confirmation: {ex.Message}");
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var apiResponse = await _apiHttpClient.PostAsJsonAsync("api/auth/forgot-password", request);
        if (apiResponse.IsSuccessStatusCode)
        {
            return Ok(new { Message = "We have sent a link to your email address to reset your password." });
        }
        else
        {
            var errorContent = await apiResponse.Content.ReadAsStringAsync();
            return StatusCode((int)apiResponse.StatusCode, errorContent);
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromQuery] string userId, [FromQuery] string token,
        [FromBody] string newPassword)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword)) return BadRequest("Some parameters are null or empty");
        ResetPasswordRequestDto password = new ResetPasswordRequestDto { Password = newPassword };
        var encodedToken = Uri.EscapeDataString(token);
        var apiResponse = await _apiHttpClient.PostAsJsonAsync($"api/auth/reset-password?userId={userId}&token={encodedToken}", password);
        if (apiResponse.IsSuccessStatusCode)
        {
            return Ok(new { message = "Success."});
        }
        else
        {
            var errorContent = await apiResponse.Content.ReadAsStringAsync();
            return StatusCode((int)apiResponse.StatusCode, errorContent);
        }
    }
    [HttpGet("data")] // This would be called by your Blazor client to get protected data
    // You'd need to add authorization here if you want to restrict access to the proxy endpoint
    // [Authorize]
    public async Task<IActionResult> GetData()
    {
        // Retrieve JWT from cookie (or session)
        var authToken = Request.Cookies["AuthToken"];

        if (string.IsNullOrEmpty(authToken))
        {
            return Unauthorized("No authentication token found.");
        }

        // Add JWT to the request to the actual API
        _apiHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

        // Call the actual API
        var apiResponse = await _apiHttpClient.GetAsync("protected-data"); // Assuming your API has a /protected-data endpoint

        if (apiResponse.IsSuccessStatusCode)
        {
            var content = await apiResponse.Content.ReadAsStringAsync();
            return Ok(content);
        }
        else
        {
            var errorContent = await apiResponse.Content.ReadAsStringAsync();
            return StatusCode((int)apiResponse.StatusCode, errorContent);
        }
    }
}