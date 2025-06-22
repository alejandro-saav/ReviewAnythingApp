using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Repositories;

public class ReviewVoteRepository : Repository<ReviewVote>, IReviewVoteRepository
{
    public ReviewVoteRepository(ReviewAnythingDbContext context) : base(context) {}
    
    public async Task<int> GetVotesByReviewIdAsync(int reviewId)
    {
        return await _context.ReviewVotes.Where(rv => rv.ReviewId == reviewId).SumAsync(rv => rv.VoteType);
    }

    public async Task<ReviewVote?> GetByUserAndReviewIdAsync(int? userId, int reviewId)
    {
        return await _context.ReviewVotes.FirstOrDefaultAsync(rv => rv.UserId == userId && rv.ReviewId == reviewId);
    }
}