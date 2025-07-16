using BlazorApp1.Models;

namespace BlazorApp1.Services;

public interface IReviewService
{
    Task<IEnumerable<Category>> GetAllReviewCategoriesAsync();
    Task<ReviewModel> CreateReviewAsync(ReviewViewModel review);

    string? LastErrorMessage { get; }

    Task<ReviewModel?> GetReviewByIdAsync(int reviewId);
    Task<ReviewPageData?> GetReviewPageDataAsync(int reviewId);
    Task<IEnumerable<Comment>> GetCommentsByReviewIdAsync(int reviewId);
    Task<Comment?> CreateCommentAsync(CreateComment comment);

    Task<bool> ReviewVoteAsync(ReviewVote vote);
    Task<bool> CommentVoteAsync(CommentVoteRequest vote);
    Task<IEnumerable<LikesReviewsModel>> GetMyReviewsAsync(ExploreQueryParams queryParams);
    Task<IEnumerable<LikesReviewsModel>> GetLikesReviewsAsync(ExploreQueryParams queryParams);

    Task<IEnumerable<MyCommentsPageModel>> GetMyCommentsPage();
    Task<IEnumerable<LikesReviewsModel>> GetExplorePageReviewsAsync(ExploreQueryParams queryParams);
    string BuildQueryString(ExploreQueryParams queryParams);
}