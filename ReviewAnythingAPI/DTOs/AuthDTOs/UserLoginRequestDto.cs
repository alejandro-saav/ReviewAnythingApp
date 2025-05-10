using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.DTOs.AuthDTOs;

public class UserLoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}