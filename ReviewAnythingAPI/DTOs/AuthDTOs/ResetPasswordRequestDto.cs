using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.DTOs.AuthDTOs;

public class ResetPasswordRequestDto
{
    [Required]
    [StringLength(50, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).+$", 
        ErrorMessage = "The password must contain uppercase and lowercase letters, numbers, and a special character.")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}