using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Repositories;

public class CommentVoteRepository : Repository<CommentVote>, ICommentVoteRepository
{
    public CommentVoteRepository(ReviewAnythingDbContext context) : base(context) {}

    public async Task<IEnumerable<int>> GetVotesByCommentIdAsync(int commentId)
    {
        return await _context.CommentVotes.Where(c => c.CommentId == commentId).Select(c => c.VoteType).ToListAsync();
    }

    public async Task<CommentVote?> GetByUserAndCommentIdAsync(int userId, int commentId)
    {
        return await _context.CommentVotes.FirstOrDefaultAsync(c => c.UserId == userId && c.CommentId == commentId);
    }
}