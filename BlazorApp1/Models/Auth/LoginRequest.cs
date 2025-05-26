using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")] // Adjust as per your API rules
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}