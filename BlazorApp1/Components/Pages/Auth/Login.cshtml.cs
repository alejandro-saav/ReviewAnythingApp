using System.Security.Claims;
using BlazorApp1.Models.Auth;
using BlazorApp1.Services;
// using BlazorApp1.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorApp1.Components.Pages.Auth;

public class Login : PageModel
{
    public bool IsLoading { get; set; }
    public string? ErrorMessage { get; set; }
    
    private readonly IAuthService _authService;
    
    [BindProperty]
    public LoginRequest LoginModel { get; set; } = new LoginRequest();


    public Login(IAuthService authService)
    {
        _authService = authService;
    }
    
    public void OnGet()
    {
        
    }

    public async Task<IActionResult> OnPostAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        if (!ModelState.IsValid)
        {
            ErrorMessage ="Invalid username or password";
            return Page();
        }

        try
        {
            var loginResponse = await _authService.LoginAsync(LoginModel);
            if (loginResponse == null || !loginResponse.Success)
            {
                ErrorMessage = "Invalid username or password";
                return Page();
            };
            var jwt = loginResponse.Token;
            Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, loginResponse.UserResponse!.UserId.ToString()),
                new Claim(ClaimTypes.Name, loginResponse.UserResponse.UserName),
                new Claim(ClaimTypes.Email, loginResponse.UserResponse.Email),
                new Claim("profile_image", loginResponse.UserResponse.ProfileImage ?? ""),
                new Claim("jwt", jwt)
            };
            
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return Redirect("/");
        }
        catch (Exception ex)
        {
            ErrorMessage = "An unexpected error occurred during login. Please try again.";
            return Page();
        }
        finally
        {
            IsLoading = false;
        }
    }
}