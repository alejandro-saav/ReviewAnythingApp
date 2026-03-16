using ReviewAnythingAPI.DTOs.UserDTOs;

namespace ReviewAnythingAPI.DTOs.AuthDTOs;

public class SuccessAuthResponseDto
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "";
    public string Token { get; set; } = "";
    public required UserResponseDto UserResponse { get; set; }
}