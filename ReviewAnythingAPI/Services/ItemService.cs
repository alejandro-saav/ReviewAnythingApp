using ReviewAnythingAPI.DTOs.ItemDTOs;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Services;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;
    private readonly ILogger<ItemService> _logger;

    public ItemService(IItemRepository itemRepository, ILogger<ItemService> logger)
    {
        _itemRepository = itemRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ItemSummaryDto>> GetItemsByCategoryIdAsync(int categoryId)
    {
        try
        {
            if (categoryId < 0)
            {
                _logger.LogWarning("Invalid category ID: {CategoryId}", categoryId);
                return Enumerable.Empty<ItemSummaryDto>();
            }
            var items = await _itemRepository.GetItemsByCategoryIdAsync(categoryId);
            return items;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving items for category {categoryId}", categoryId);
            throw;
        }
    }

    public async Task<IEnumerable<ItemSummaryDto>> GetItemsByUserIdAsync(int userId)
    {
        try
        {
            if (userId < 0)
            {
                _logger.LogWarning("Invalid user ID: {UserId}", userId);
                return Enumerable.Empty<ItemSummaryDto>();
            }
            var items = await _itemRepository.GetItemsByUserIdAsync(userId);
            return items != null ? items : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving items for userId {userId}", userId);
            throw;
        }
    }

    public async Task<ItemSummaryDto?> GetItemByNameAsync(string itemName)
    {
        try
        {
            if (string.IsNullOrEmpty(itemName))
            {
                _logger.LogWarning("Invalid item name: {ItemName}", itemName);
                return null;
            }
            var item = await _itemRepository.GetItemByItemNameAsync(itemName);
            return item;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving item name: {ItemName}", itemName);
            throw;
        }
    }
}