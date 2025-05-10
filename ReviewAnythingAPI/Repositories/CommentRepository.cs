using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Repositories;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(ReviewAnythingDbContext context) : base(context)
    {}

    public async Task<IEnumerable<Comment>> GetAllCommentsByReviewIdAsync(int reviewId)
    {
        return await _context.Comments.Where(c => c.ReviewId == reviewId).ToListAsync();
    }

    public async Task<IEnumerable<Comment>> GetAllCommentsByUserIdAsync(int userId)
    {
        return await _context.Comments.Where(c => c.UserId == userId).ToListAsync();
    }
}