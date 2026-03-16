using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.DTOs.AuthDTOs;

public class UserLoginRequestDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    public required string Password { get; set; }
}