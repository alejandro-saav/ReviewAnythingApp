using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.DTOs.AuthDTOs;

public class UserRegistrationRequestDto
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
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$", 
        ErrorMessage = "The password must contain uppercase and lowercase letters, numbers, and a special character.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Phone]
    public string? Phone { get; set; }
    public IFormFile? ProfileImage { get; set; }
    [StringLength(500)]
    [DataType(DataType.MultilineText)]
    public string? Bio { get; set; }
}