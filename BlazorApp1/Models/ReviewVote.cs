namespace BlazorApp1.Models;

public class ReviewVote
{
    public int ReviewId { get; set; }
    public int VoteType { get; set; }
    public string jwtToken { get; set; } = string.Empty;
}