using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Repositories.Interfaces;

public interface ICommentVoteRepository : IRepository<CommentVote>
{
    public Task<IEnumerable<int>> GetVotesByCommentIdAsync(int commentId);
}