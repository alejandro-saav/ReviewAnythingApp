using BlazorApp1.Models.Auth;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorApp1.Components.Pages.Auth;

public class Login : PageModel
{
    public bool IsLoading { get; set; }
    public string? ErrorMessage { get; set; }
    
    private readonly IAuthService _authService;
    
    [BindProperty]
    public LoginRequest LoginModel { get; set; }

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
            var success = await _authService.LoginAsync(LoginModel);
            if (success)
            {
                return Redirect("/test");
            }
            else
            {
                ErrorMessage = "Invalid username or password";
                return Page();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            ErrorMessage = "An unexpected error occurred during login. Please try again.";
            return Page();
        }
        finally
        {
            IsLoading = false;
        }
    }
}