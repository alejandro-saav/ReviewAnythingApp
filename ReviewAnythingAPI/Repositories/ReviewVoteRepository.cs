using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Repositories;

public class ReviewVoteRepository : Repository<ReviewVote>, IReviewVoteRepository
{
    public ReviewVoteRepository(ReviewAnythingDbContext context) : base(context) {}
    
    public async Task<IEnumerable<int>> GetVotesByReviewIdAsync(int reviewId)
    {
        return await _context.ReviewVotes.Where(rv => rv.ReviewId == reviewId).Select(rv => rv.VoteType).ToListAsync();
    }
}