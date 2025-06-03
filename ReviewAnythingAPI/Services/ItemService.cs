using ReviewAnythingAPI.DTOs.ItemDTOs;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Services;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;
    private readonly IRepository<Category> _categoryRepository;

    public ItemService(IItemRepository itemRepository, IRepository<Category> categoryRepository)
    {
        _itemRepository = itemRepository;
        _categoryRepository = categoryRepository;
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

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories;
    }
}