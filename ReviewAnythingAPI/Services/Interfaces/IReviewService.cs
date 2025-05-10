using ReviewAnythingAPI.DTOs.ReviewDTOs;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Services.Interfaces;

public interface IReviewService
{
    Task<IEnumerable<ReviewResponseDto>> GetAllReviewsByUserIdAsync(int userId);
    
    Task<IEnumerable<ReviewResponseDto>> GetAllReviewsByItemIdAsync(int itemId);

    Task<ReviewResponseDto> CreateReviewAsync(ReviewCreateRequestDto review, int userId);
}