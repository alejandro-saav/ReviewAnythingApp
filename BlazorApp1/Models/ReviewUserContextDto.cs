namespace BlazorApp1.Models;

public class ReviewUserContextDto
{
    public int? UserReviewVote { get; set; } = null;
    public IEnumerable<CommentVoteResponse> CommentVotes { get; set; } = [];
    public IEnumerable<int> FollowedUserIds { get; set; } = [];
}