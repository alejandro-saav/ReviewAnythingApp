using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Repositories.Interfaces;

public interface IItemRepository : IRepository<Item>
{
    public Task<IEnumerable<Item>> GetItemsByCategoryIdAsync(int categoryId);

    public Task<Item> GetItemByItemNameAsync(string itemName);
    
    public Task<IEnumerable<Item>> GetItemsByUserIdAsync(int userId);
}