using BlazorApp1.Models;

namespace BlazorApp1.Services;

public interface IReviewService
{
    Task<IEnumerable<Category>> GetAllReviewCategoriesAsync();
    
    string? LastErrorMessage { get; }
}