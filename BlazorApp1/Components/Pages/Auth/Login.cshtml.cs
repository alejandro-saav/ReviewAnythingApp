using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using BlazorApp1.Models.Auth;
using BlazorApp1.Services;
// using BlazorApp1.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorApp1.Components.Pages.Auth;

public class Login : PageModel
{
    private readonly IConfiguration _configuration;
    public string GoogleClientId { get; private set; }
    public bool IsLoading { get; set; }
    public string? ErrorMessage { get; set; }
    public string AuthUrl { get; set; } = "";

    private readonly IAuthService _authService;
    private readonly GoogleOAuthService _googleOAuthService;

    [BindProperty] public LoginRequest LoginModel { get; set; } = new LoginRequest();


    public Login(IAuthService authService, IConfiguration configuration, GoogleOAuthService googleOAuthService)
    {
        _authService = authService;
        _configuration = configuration;
        _googleOAuthService = googleOAuthService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        GoogleClientId = _configuration["Google:ClientId"] ??
                         throw new InvalidOperationException("Google ClientId not set");
        var code = Request.Query["code"].FirstOrDefault();
        var error = Request.Query["error"].FirstOrDefault();
        var redirectUri = "http://localhost:5002/login";
        AuthUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                                 $"client_id={GoogleClientId}&" +
                                 $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                                 $"response_type=code&" +
                                 $"scope=openid%20profile%20email&" +
                                 $"access_type=offline&" + $"prompt=select_account";
        if (!string.IsNullOrEmpty(error))
        {
            Console.WriteLine("Error google code error:" + error);
        }

        if (string.IsNullOrEmpty(code))
        {
            Console.WriteLine("Google code is null or empty");
        }
        else
        {
            try
            {
                var googleTokenResponse = await _googleOAuthService.ExchangeCodeForTokenAsync(code);
                if (string.IsNullOrEmpty(googleTokenResponse.IdToken))
                {
                    ErrorMessage = "Token not found";
                }

                var loginResponse = await _authService.GoogleLoginAsync(googleTokenResponse.IdToken);
                if (loginResponse == null || !loginResponse.Success)
                {
                    ErrorMessage = "Invalid username or password";
                    return Page();
                }

                ;
                var jwt = loginResponse.Token;
                Response.Cookies.Append("jwt", jwt, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                });
                var principal = GetClaimsFromToken(jwt);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return Redirect("/");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }

        return Page();
    }

    public ClaimsPrincipal GetClaimsFromToken(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(jwtToken)) throw new ArgumentException("Token is not valid");
        var token = handler.ReadJwtToken(jwtToken);

        var identity = new ClaimsIdentity(token.Claims, "jwt");
        var principal = new ClaimsPrincipal(identity);
        return principal;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Invalid username or password";
            return Page();
        }

        try
        {
            var loginResponse = await _authService.LoginAsync(LoginModel);
            if (loginResponse == null || !loginResponse.Success)
            {
                ErrorMessage = "Invalid username or password";
                return Page();
            }

            ;
            var jwt = loginResponse.Token;
            Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });

            // var claims = new List<Claim>
            // {
            //     new Claim(ClaimTypes.NameIdentifier, loginResponse.UserResponse!.UserId.ToString()),
            //     new Claim(ClaimTypes.Name, loginResponse.UserResponse.UserName),
            //     new Claim(ClaimTypes.Email, loginResponse.UserResponse.Email),
            //     new Claim("profile_image", loginResponse.UserResponse.ProfileImage ?? ""),
            //     new Claim("jwt", jwt)
            // };
            //
            // var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            // var principal = new ClaimsPrincipal(identity);
            var principal = GetClaimsFromToken(jwt);
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