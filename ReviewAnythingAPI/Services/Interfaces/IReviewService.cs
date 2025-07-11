using ReviewAnythingAPI.DTOs.CommentDTOs;
using ReviewAnythingAPI.DTOs.ReviewDTOs;
using ReviewAnythingAPI.HelperClasses;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Services.Interfaces;

public interface IReviewService
{
    Task<IEnumerable<ReviewDetailDto>> GetAllReviewsByUserIdAsync(int userId);

    Task<IEnumerable<ReviewDetailDto>> GetAllReviewsByItemIdAsync(int itemId);

    Task<ReviewSummaryDto> CreateReviewAsync(ReviewCreateRequestDto reviewCreateRequestDto, int userId);

    Task<ReviewSummaryDto> UpdateReviewAsync(ReviewUpdateRequestDto reviewUpdateRequestDto, int userId, int reviewId);
    
    Task<ReviewDetailDto?> GetReviewByIdAsync(int reviewId);
    
    Task<bool> DeleteReviewAsync(int reviewId, int userId);
    
    Task<ReviewVoteResponseDto> ReviewVoteAsync(ReviewVoteRequestDto reviewVoteRequestDto, int userId);
    Task<ReviewPageDataDto> GetReviewPageDataAsync(int? userId, int reviewId);
    Task<IEnumerable<MyReviewsDto>> GetMyReviewsAsync(int userId);
    Task<IEnumerable<LikesReviewsDto>> GetLikesReviewsAsync(int userId);
    Task<IEnumerable<LikesReviewsDto>> GetExplorePageReviewsAsync(ExploreQueryParamsDto queryParamsDto, int pageSize);
}