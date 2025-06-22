using ReviewAnythingAPI.DTOs.ReviewDTOs;
using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Repositories.Interfaces;

public interface IReviewRepository : IRepository<Review>
{
    public Task<IEnumerable<Review>> GetAllReviewsByUserIdAsync(int userId);

    public Task<IEnumerable<Review>> GetAllReviewsByItemIdAsync(int itemId);
    
    public Task<Review> GetReviewByUserIdAndItemIdAsync(int userId, int itemId);
    public Task<ReviewDetailDto?> GetReviewDetailByIdAsync(int reviewId);
}