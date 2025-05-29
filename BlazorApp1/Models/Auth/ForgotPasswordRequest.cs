using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models.Auth;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}