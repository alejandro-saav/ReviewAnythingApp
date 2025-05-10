namespace ReviewAnythingAPI.DTOs.UserDTOs;

public class UserProfileDto
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ProfileImage { get; set; }
    public string Bio { get; set; }
    public int ReviewsWritten { get; set; }
    public int Followers { get; set; }
    public int Following { get; set; }
    public DateTime CreationDate { get; set; }
}