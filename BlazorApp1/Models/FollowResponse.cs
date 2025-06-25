namespace BlazorApp1.Models;

public class FollowResponse
{
    public int UserId { get; set; }
    public int TargetUserId { get; set; }
    public DateTime FollowDateTime { get; set; }
}