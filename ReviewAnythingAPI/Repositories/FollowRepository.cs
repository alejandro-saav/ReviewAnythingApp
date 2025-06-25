using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Repositories;

public class FollowRepository : Repository<Follow>, IFollowRepository
{
    public FollowRepository(ReviewAnythingDbContext context) : base(context){}
    
    public async Task<IEnumerable<ApplicationUser>> GetUserFollowersAsync(int userId)
    {
        return await _context.Follows.Where(f => f.FollowerUserId == userId).Select(f => f.Follower!).ToListAsync();
    }

    public async Task<IEnumerable<int>> GetUserFollowingsIdsAsync(int userId)
    {
        return await _context.Follows.Where(f => f.FollowerUserId == userId).Select(f => f.FollowingUserId!.Value)
            .ToListAsync();
    }

    public async Task<Follow?> GetEntityByIdAsync(int userId, int targetId)
    {
        var entity = await _dbSet.FindAsync(userId, targetId);
        return entity;
    }
}