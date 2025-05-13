using ReviewAnythingAPI.DTOs.ReviewDTOs;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Services.Interfaces;

public interface IReviewService
{
    Task<IEnumerable<ReviewDetailDto>> GetAllReviewsByUserIdAsync(int userId);

    Task<IEnumerable<ReviewDetailDto>> GetAllReviewsByItemIdAsync(int itemId);

    Task<ReviewSummaryDto> CreateReviewAsync(ReviewCreateRequestDto reviewCreateRequestDto, int userId);

    Task<ReviewSummaryDto> UpdateReviewAsync(ReviewUpdateRequestDto reviewUpdateRequestDto, int userId, int reviewId);
    
    Task<ReviewDetailDto> GetReviewByIdAsync(int reviewId);
    
    Task<bool> DeleteReviewAsync(int reviewId, int userId);
}