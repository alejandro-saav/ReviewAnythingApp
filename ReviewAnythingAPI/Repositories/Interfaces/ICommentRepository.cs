using ReviewAnythingAPI.DTOs.CommentDTOs;
using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Repositories.Interfaces;

public interface ICommentRepository : IRepository<Comment>
{
    public Task<IEnumerable<CommentResponseDto>> GetAllCommentsByReviewIdAsync(int reviewId);

    public Task<IEnumerable<CommentResponseDto>> GetAllCommentsByUserIdAsync(int userId);
    public Task<CommentResponseDto> CreateCommentAsync(Comment comment);
}