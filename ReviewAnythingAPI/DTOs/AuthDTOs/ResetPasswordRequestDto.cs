using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.DTOs.AuthDTOs;

public class ResetPasswordRequestDto
{
    [Required]
    public string UserId { get; set; }
    [Required]
    public string Token { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}