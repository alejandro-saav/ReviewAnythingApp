using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Repositories.Interfaces;

public interface IReviewTagRepository : IRepository<ReviewTag>
{
    public Task<IEnumerable<Tag>> GetTagsByReviewIdAsync(int reviewId);

    public Task<IEnumerable<Review>> GetAllReviewsByTagAsync(int tagId);

    public Task<IEnumerable<ReviewTag>> AddRangeAsync(IEnumerable<ReviewTag> reviewTags);

    public Task<bool> DeleteAllTagsByReviewIdAsync(int reviewId);
}