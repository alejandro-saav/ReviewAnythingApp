using ReviewAnythingAPI.DTOs.CommentDTOs;
using ReviewAnythingAPI.Repositories.Interfaces;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<CommentService> _logger;

    public async Task<IEnumerable<CommentResponseDto>> GetAllCommentsByUserAsync(int userId)
    {
        try
        {
            if (userId < 0)
            {
                _logger.LogError("User id cannot be negative");
                return Enumerable.Empty<CommentResponseDto>();
            }
            var comments = await _commentRepository.GetAllCommentsByUserIdAsync(userId);
            return comments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving comments for userId {userId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<CommentResponseDto>> GetAllCommentsByReviewAsync(int reviewId)
    {
        try
        {
            if (reviewId < 0)
            {
                _logger.LogError("Review id cannot be negative");
                return Enumerable.Empty<CommentResponseDto>();
            }
            var comments = await _commentRepository.GetAllCommentsByReviewIdAsync(reviewId);
            return comments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving comments for reviewId {reviewId}", reviewId);
            throw;
        }
    }
}