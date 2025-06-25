namespace BlazorApp1.Models;

public class FollowRequest
{
    public int TargetUserId { get; set; }
    public string JwtToken { get; set; }
}