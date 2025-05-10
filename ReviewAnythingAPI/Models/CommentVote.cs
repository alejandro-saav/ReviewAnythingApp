namespace ReviewAnythingAPI.Models;

public class CommentVote
{
    public int? UserId { get; set; }
    public int CommentId { get; set; }
    public int VoteType { get; set; }
    public DateTime VoteDate { get; set; }
    
    // Navigation Properties
    public ApplicationUser? User { get; set; }
    public Comment Comment { get; set; }
}