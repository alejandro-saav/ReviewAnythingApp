using System.Security.Claims;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorApp1.Components.Pages;

public class UpdateClaims : PageModel
{
    private readonly IUserService _userService;
    private readonly CustomAuthStateProvider _authStateProvider;

    public UpdateClaims(IUserService userService, CustomAuthStateProvider authStateProvider)
    {
        _userService = userService;
        _authStateProvider = authStateProvider;
    }
    public async Task<IActionResult> OnGet(string? returnUrl)
    {
        var user = HttpContext.User;
        if (!user.Identity.IsAuthenticated) return Redirect(returnUrl ?? "/");
        
        var jwt = user.FindFirst("jwt")?.Value;
        var email = user.FindFirst(ClaimTypes.Email)?.Value;

        try
        {
            var userSummary = await _userService.GetUserSummaryAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userSummary.UserId.ToString()),
                new Claim(ClaimTypes.Name, userSummary.UserName),
                new Claim(ClaimTypes.Email, email),
                new Claim("profile_image", userSummary.ProfileImage ?? ""),
                new Claim("jwt", jwt),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            _authStateProvider.Notify();
            return LocalRedirect(returnUrl ?? "/");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error on UpdateClaimsPage:" + ex.Message);
            return LocalRedirect(returnUrl ?? "/");
        }
    }
}