using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Repositories;

public class ItemRepository : Repository<Item>, IItemRepository
{
    public ItemRepository(ReviewAnythingDbContext context) : base(context){}

    public async Task<IEnumerable<Item>> GetItemsByCategoryIdAsync(int categoryId)
    {
        return await _context.Items.Where(i => i.CategoryId == categoryId).ToListAsync();
    }

    public async Task<Item> GetItemByItemNameAsync(string itemName)
    {
        return await _context.Items.FirstOrDefaultAsync(i => i.ItemName == itemName);
    }

    public async Task<IEnumerable<Item>> GetItemsByUserIdAsync(int userId)
    {
        return await _context.Items.Where(i => i.CreatedByUserId == userId).ToListAsync();
    }
}