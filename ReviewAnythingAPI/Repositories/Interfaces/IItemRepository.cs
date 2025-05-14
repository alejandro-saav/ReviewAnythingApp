using ReviewAnythingAPI.DTOs.ItemDTOs;
using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Repositories.Interfaces;

public interface IItemRepository : IRepository<Item>
{
    public Task<IEnumerable<ItemSummaryDto>> GetItemsByCategoryIdAsync(int categoryId);

    public Task<ItemSummaryDto> GetItemByItemNameAsync(string itemName);
    
    public Task<IEnumerable<ItemSummaryDto>> GetItemsByUserIdAsync(int userId);
}