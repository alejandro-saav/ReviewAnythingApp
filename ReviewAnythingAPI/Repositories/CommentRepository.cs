using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.DTOs.CommentDTOs;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Repositories;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(ReviewAnythingDbContext context) : base(context)
    {}

    public async Task<IEnumerable<CommentResponseDto>> GetAllCommentsByReviewIdAsync(int reviewId)
    {
        return await _context.Comments.Where(c => c.ReviewId == reviewId).Select(c => new CommentResponseDto
        {
            CommentId = c.CommentId,
            Content = c.Content,
            CreatorUserName = c.User.UserName,
            LastEditDate = c.LastEditDate,
            ReviewId = c.ReviewId
        }).ToListAsync();
    }

    public async Task<IEnumerable<CommentResponseDto>> GetAllCommentsByUserIdAsync(int userId)
    {
        return await _context.Comments.Where(c => c.UserId == userId).Select(c => new CommentResponseDto
        {
            CommentId = c.CommentId,
            Content = c.Content,
            CreatorUserName = c.User.UserName,
            LastEditDate = c.LastEditDate,
            ReviewId = c.ReviewId
        }).ToListAsync();
    }
}