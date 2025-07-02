namespace BlazorApp1.Models;

public class CommentVoteRequest
{
    public int CommentId { get; set; }
    public int ReviewId { get; set; }
    public int VoteType { get; set; }
}