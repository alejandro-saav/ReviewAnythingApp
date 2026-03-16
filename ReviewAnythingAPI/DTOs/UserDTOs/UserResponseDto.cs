namespace ReviewAnythingAPI.DTOs.UserDTOs;

public class UserResponseDto
{
    public required int UserId { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Phone { get; set; }
    public required string ProfileImage { get; set; }
    public required string Bio { get; set; }
    public required DateTime? CreationDate { get; set; }
}