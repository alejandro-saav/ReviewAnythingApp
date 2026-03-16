using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.DTOs.AuthDTOs;

public class ForgotPasswordRequestDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
}