using ReviewAnythingAPI.DTOs.ItemDTOs;
using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Services.Interfaces;

public interface IItemService
{
    Task<IEnumerable<ItemSummaryDto>> GetItemsByCategoryIdAsync(int categoryId);

    Task<IEnumerable<ItemSummaryDto>> GetItemsByUserIdAsync(int userId);

    Task<ItemSummaryDto?> GetItemByNameAsync(string itemName);
}