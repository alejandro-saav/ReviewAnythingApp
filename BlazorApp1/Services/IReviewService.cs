using BlazorApp1.Models;

namespace BlazorApp1.Services;

public interface IReviewService
{
    Task<IEnumerable<Category>> GetAllReviewCategoriesAsync();
    Task<object> CreateReviewAsync(ReviewViewModel review, string jwt);
    
    string? LastErrorMessage { get; }
}