using ReviewAnythingAPI.DTOs.ReviewDTOs;
using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Repositories.Interfaces;

public interface ICommentVoteRepository : IRepository<CommentVote>
{
    public Task<IEnumerable<int>> GetVotesByCommentIdAsync(int commentId);
    public Task<CommentVote?> GetByUserAndCommentIdAsync(int userId, int reviewId);
    public Task<IEnumerable<CommentVoteResponseDto>> GetVotesByReviewIdAndUserIdAsync(int reviewId, int userId);
}