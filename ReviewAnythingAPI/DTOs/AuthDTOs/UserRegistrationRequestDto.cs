using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.DTOs.AuthDTOs;

public class UserRegistrationRequestDto
{
    [Required]
    [StringLength(20)]
    public string Username { get; set; }
    [Required]
    [StringLength(30)]
    public string FirstName { get; set; }
    [StringLength(30)]
    public string LastName { get; set; }
    [Required] 
    [EmailAddress] 
    public string Email { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required]
    [Compare("Password")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
    [Phone]
    public string Phone { get; set; }
    [Url]
    [RegularExpression(@".*\.(gif|jpe?g|bmp|png)$", ErrorMessage = "URL must end with a valid image extension (.jpg, .jpeg, .png, .gif, or .bmp)")]
    [DataType(DataType.ImageUrl)]
    public string? ProfileImage { get; set; }
    [StringLength(500)]
    [DataType(DataType.MultilineText)]
    public string Bio { get; set; }
}