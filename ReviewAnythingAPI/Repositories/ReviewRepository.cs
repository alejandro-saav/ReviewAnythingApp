using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Repositories;

public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(ReviewAnythingDbContext context) : base(context)
    {}

    public async Task<IEnumerable<Review>> GetAllReviewsByUserIdAsync(int userId)
    {
        return await _context.Reviews.Where(r => r.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetAllReviewsByItemIdAsync(int itemId)
    {
        return await _context.Reviews.Where(r => r.ItemId == itemId).ToListAsync();
    }
}