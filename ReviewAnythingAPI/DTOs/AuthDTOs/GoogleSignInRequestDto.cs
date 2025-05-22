using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.DTOs.AuthDTOs;

public class GoogleSignInRequestDto
{
    [Required]
    public string IdToken { get; set; }
}