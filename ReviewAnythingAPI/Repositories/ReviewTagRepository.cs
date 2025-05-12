using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Repositories;

public class ReviewTagRepository : Repository<ReviewTag>, IReviewTagRepository
{
    public ReviewTagRepository(ReviewAnythingDbContext context) : base(context){}
    
    public async Task<IEnumerable<Tag>> GetTagsByReviewIdAsync(int reviewId)
    {
        return await _context.ReviewTags.Where(rt => rt.ReviewId == reviewId).Select(rt => rt.Tag).ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetAllReviewsByTagAsync(int tagId)
    {
        return await _context.ReviewTags.Where(rt => rt.TagId == tagId).Select(rt => rt.TagReview).ToListAsync();
    }

    public async Task<IEnumerable<ReviewTag>> AddRangeAsync(IEnumerable<ReviewTag> reviewTags)
    {
        await _context.ReviewTags.AddRangeAsync(reviewTags);
        //await _context.SaveChangesAsync();
        return reviewTags;
    }

    public async Task<bool> DeleteAllTagsByReviewIdAsync(int reviewId)
    {
        var rowsAffected = await _context.ReviewTags.Where(rt => rt.ReviewId == reviewId).ExecuteDeleteAsync();
        return rowsAffected > 0;
    }
}