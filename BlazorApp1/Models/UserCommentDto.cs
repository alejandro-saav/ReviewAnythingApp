namespace BlazorApp1.Models;

public class UserCommentDto
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string? ProfileImage { get; set; }
    public int ReviewCount { get; set; }
    public int FollowerCount { get; set; }
}