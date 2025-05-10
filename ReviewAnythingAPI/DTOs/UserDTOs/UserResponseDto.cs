namespace ReviewAnythingAPI.DTOs.UserDTOs;

public class UserResponseDto
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public string ProfileImage { get; set; }
    public string Bio { get; set; }
    public DateTime? CreationDate { get; set; }
}