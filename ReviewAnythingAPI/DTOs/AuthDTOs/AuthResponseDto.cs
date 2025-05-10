using ReviewAnythingAPI.DTOs.UserDTOs;

namespace ReviewAnythingAPI.DTOs.AuthDTOs;

public class AuthResponseDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Token { get; set; }
    public UserResponseDto? UserResponse { get; set; }
    public List<string>? Errors { get; set; }
}