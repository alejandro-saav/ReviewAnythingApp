using ReviewAnythingAPI.DTOs.ItemDTOs;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Services;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;

    public ItemService(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<IEnumerable<ItemSummaryDto>> GetItemsByCategoryIdAsync(int categoryId)
    {
            var items = await _itemRepository.GetItemsByCategoryIdAsync(categoryId);
            return items;
        
    }

    public async Task<IEnumerable<ItemSummaryDto>> GetItemsByUserIdAsync(int userId)
    {
            var items = await _itemRepository.GetItemsByUserIdAsync(userId);
            return items;
    }

    public async Task<ItemSummaryDto?> GetItemByNameAsync(string itemName)
    {
            var item = await _itemRepository.GetItemByItemNameAsync(itemName);
            return item;
    }
}