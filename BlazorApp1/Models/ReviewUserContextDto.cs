namespace BlazorApp1.Models;

public class ReviewUserContextDto
{
    public int? UserReviewVote { get; set; } = null;
    public List<CommentVoteResponse> CommentVotes { get; set; } = [];
    public List<int> FollowedUserIds { get; set; } = [];
}