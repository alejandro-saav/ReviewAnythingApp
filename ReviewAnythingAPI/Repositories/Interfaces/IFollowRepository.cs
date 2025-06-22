using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Repositories.Interfaces;

public interface IFollowRepository : IRepository<Follow>
{
    public Task<IEnumerable<ApplicationUser>> GetUserFollowersAsync(int userId);
    
    public Task<IEnumerable<int>> GetUserFollowingsIdsAsync(int userId);
}