using ReviewAnythingAPI.DTOs.CommentDTOs;
using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Services.Interfaces;

public interface ICommentService
{
    public Task<IEnumerable<CommentResponseDto>> GetAllCommentsByUserAsync(int userId);
    public Task<IEnumerable<CommentResponseDto>> GetAllCommentsByReviewAsync(int reviewId);
}