using BlazorApp1.Models;
using ReviewAnythingAPI.Models;
using Category = BlazorApp1.Models.Category;
using Comment = BlazorApp1.Models.Comment;

namespace BlazorApp1.Services;

public interface IReviewService
{
    Task<IEnumerable<Category>> GetAllReviewCategoriesAsync();
    Task<ReviewModel> CreateReviewAsync(ReviewViewModel review);
    
    string? LastErrorMessage { get; }
    
    public ReviewModel? CreatedReview { get; set; }
    Task<ReviewModel> GetReviewByIdAsync(int reviewId);
    Task<IEnumerable<Comment>> GetCommentsByReviewIdAsync(int reviewId);
}