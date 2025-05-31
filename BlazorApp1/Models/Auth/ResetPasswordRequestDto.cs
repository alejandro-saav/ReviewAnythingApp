using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models.Auth;

public class ResetPasswordRequestDto
{
    [Required]
    [StringLength(50, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$", 
        ErrorMessage = "The password must contain uppercase and lowercase letters, numbers, and a special character.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}