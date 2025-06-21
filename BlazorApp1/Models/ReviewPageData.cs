namespace BlazorApp1.Models;

public class ReviewPageData
{
    public ReviewModel Review { get; set; } = new();
    public IEnumerable<Comment> Comments { get; set; } = [];
    public bool HasUserLikedReview { get; set; } = false;
    public IEnumerable<int> LikedCommentIds { get; set; } = [];
    public IEnumerable<int> FollowedUserIds { get; set; } = [];
}