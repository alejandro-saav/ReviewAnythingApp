using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Repositories.Interfaces;

public interface IReviewVoteRepository : IRepository<ReviewVote>
{
    public Task<IEnumerable<int>> GetVotesByReviewIdAsync(int reviewId);
}