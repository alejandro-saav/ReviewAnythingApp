namespace ReviewAnythingAPI.DTOs.UserDTOs;

public class UserSummaryDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = "";
    public string? ProfileImage { get; set; }
    public string FirstName { get; set; } = "";
    public DateTime? CreationDate { get; set; }
    public string LastName { get; set; } = "";
    public string Bio { get; set; } = "";
}