using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.DTOs.AuthDTOs;

public class GoogleSignInRequestDto
{
    [Required]
    public required string IdToken { get; set; }
}