using ReviewAnythingAPI.DTOs.CommentDTOs;
using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Services.Interfaces;

public interface ICommentService
{
    public Task<IEnumerable<CommentResponseDto>> GetAllCommentsByUserAsync(int userId);
    public Task<IEnumerable<CommentResponseDto>> GetAllCommentsByReviewAsync(int reviewId);
    public Task<CommentResponseDto> CreateCommentAsync(CommentCreateRequestDto commentCreateRequestDto, int userId, string userName);
    
    public Task<CommentResponseDto> UpdateCommentAsync(CommentCreateRequestDto commentCreateRequestDto, int commentId, int userId, string userName);
    public Task<CommentResponseDto> GetCommentByIdAsync(int commentId);
    public Task DeleteCommentByIdAsync(int commentId, int userId);
}