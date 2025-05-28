using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models.Auth;

public class RegisterRequestDto
{
    [Required]
    [StringLength(20)]
    public string UserName { get; set; }
    [Required]
    [StringLength(30)]
    public string FirstName { get; set; }
    [StringLength(30)]
    public string? LastName { get; set; }
    [Required] 
    [EmailAddress] 
    public string Email { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Phone]
    public string? Phone { get; set; }
    public string? ProfileImage { get; set; }
    [StringLength(500)]
    [DataType(DataType.MultilineText)]
    public string? Bio { get; set; }
}