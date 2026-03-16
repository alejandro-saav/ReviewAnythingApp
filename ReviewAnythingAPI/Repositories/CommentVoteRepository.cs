using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.DTOs.ReviewDTOs;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Repositories;

public class CommentVoteRepository : Repository<CommentVote>, ICommentVoteRepository
{
    public CommentVoteRepository(ReviewAnythingDbContext context) : base(context) {}

    public async Task<IEnumerable<int>> GetVotesByCommentIdAsync(int commentId)
    {
        return await _context.CommentVotes.AsNoTracking().Where(c => c.CommentId == commentId).Select(c => c.VoteType).ToListAsync();
    }

    public async Task<CommentVote?> GetByUserAndCommentIdAsync(int userId, int commentId)
    {
        return await _context.CommentVotes.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == userId && c.CommentId == commentId);
    }

    public async Task<IEnumerable<CommentVoteResponseDto>> GetVotesByReviewIdAndUserIdAsync(int reviewId, int userId)
    {
        return await _context.CommentVotes.AsNoTracking().Where(cv => cv.UserId == userId && cv.ReviewId == reviewId).Select(cv =>
            new CommentVoteResponseDto
            {
                UserVote = cv.VoteType,
                CommentId = cv.CommentId,
            }).ToListAsync();
    }
}