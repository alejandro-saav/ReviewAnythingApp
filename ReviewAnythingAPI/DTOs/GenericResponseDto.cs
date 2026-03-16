using ReviewAnythingAPI.DTOs.UserDTOs;

namespace ReviewAnythingAPI.DTOs.AuthDTOs;

public class GenericResponseDto
{
    public bool Success { get; set; }
    public required string Message { get; set; }
}