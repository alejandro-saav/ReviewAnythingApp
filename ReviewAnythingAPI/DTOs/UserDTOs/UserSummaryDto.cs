namespace ReviewAnythingAPI.DTOs.UserDTOs;

public class UserSummaryDto
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string? ProfileImage { get; set; }
    public string FirstName { get; set; }
    public string Email { get; set; }
}