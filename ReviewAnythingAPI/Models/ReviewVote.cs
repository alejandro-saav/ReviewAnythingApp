namespace ReviewAnythingAPI.Models;

public class ReviewVote
{
    public int? UserId { get; set; }
    public int ReviewId { get; set; }
    public int VoteType { get; set; }
    public DateTime VoteDate { get; set; }
    
    // Navigation Properties
    public ApplicationUser? User { get; set; }
    public Review Review { get; set; }
}