using ReviewAnythingAPI.DTOs.ReviewDTOs;
using ReviewAnythingAPI.HelperClasses;
using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Repositories.Interfaces;

public interface IReviewRepository : IRepository<Review>
{
    public Task<IEnumerable<Review>> GetAllReviewsByUserIdAsync(int userId);

    public Task<IEnumerable<Review>> GetAllReviewsByItemIdAsync(int itemId);

    public Task<Review> GetReviewByUserIdAndItemIdAsync(int userId, int itemId);
    public Task<ReviewDetailDto?> GetReviewDetailByIdAsync(int reviewId);
    public Task<IEnumerable<LikesReviewsDto>> GetMyReviewsAsync(int userId, int pageSize, ExploreQueryParamsDto queryParams);
    public Task<IEnumerable<LikesReviewsDto>> GetLikesReviewsAsync(int userId, int pageSize, ExploreQueryParamsDto queryParams);
    public Task<IEnumerable<LikesReviewsDto>> GetExplorePageReviewsAsync(ExploreQueryParamsDto queryParamsDto, int pageSize);

    public IQueryable<Review> FilterReviews(ExploreQueryParamsDto queryParamsDto, int? userId = null, int? likedByUserId = null);
}