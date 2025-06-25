namespace BlazorApp1.Models;

public class ReviewPageData
{
    public ReviewModel Review { get; set; } = new();
    public List<Comment> Comments { get; set; } = [];
    public int? UserReviewVote { get; set; } = null;
    public List<CommentVoteResponse> CommentVotes { get; set; } = [];
    public List<int> FollowedUserIds { get; set; } = [];
}