using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Repositories.Interfaces;

public interface ICommentRepository : IRepository<Comment>
{
    public Task<IEnumerable<Comment>> GetAllCommentsByReviewIdAsync(int reviewId);

    public Task<IEnumerable<Comment>> GetAllCommentsByUserIdAsync(int userId);
}