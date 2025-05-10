namespace ReviewAnythingAPI.Models;

public class Follow
{
    public int? FollowerUserId { get; set; }
    public int? FollowingUserId { get; set; }
    public DateTime FollowDate { get; set; }
    
    // Navigation Properties
    public ApplicationUser? Follower { get; set; }
    public ApplicationUser? Following { get; set; }
}