using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Repositories.Interfaces;

public interface IReviewVoteRepository : IRepository<ReviewVote>
{
    public Task<int> GetVotesByReviewIdAsync(int reviewId);
    
    public Task<ReviewVote?> GetByUserAndReviewIdAsync(int? userId, int reviewId);
}