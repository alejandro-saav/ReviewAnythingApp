using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.DTOs.ItemDTOs;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Repositories;

public class ItemRepository : Repository<Item>, IItemRepository
{
    public ItemRepository(ReviewAnythingDbContext context) : base(context){}

    public async Task<IEnumerable<ItemSummaryDto>> GetItemsByCategoryIdAsync(int categoryId)
    {
        return await _context.Items.Where(item => item.CategoryId == categoryId).Select(item => new ItemSummaryDto
        {
            ItemId = item.ItemId,
            ItemName = item.ItemName,
            TotalReviews = item.ItemReviews.Count(),
            AvgRating = item.ItemReviews.Any() ? (int)Math.Round(item.ItemReviews.Average(r => r.Rating)) : 0,
            CreatedBy = item.Creator.UserName,
            CreationDate = item.CreationDate,
        }).ToListAsync();
    }

    public async Task<ItemSummaryDto> GetItemByItemNameAsync(string itemName)
    {
        return await _context.Items.Where(i => i.ItemName == itemName).Select(item => new ItemSummaryDto
        {
            ItemId = item.ItemId,
            ItemName = item.ItemName,
            TotalReviews = item.ItemReviews.Count(),
            AvgRating = item.ItemReviews.Any() ? (int)Math.Round(item.ItemReviews.Average(r => r.Rating)) : 0,
            CreatedBy = item.Creator.UserName,
            CreationDate = item.CreationDate,
        }).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ItemSummaryDto>> GetItemsByUserIdAsync(int userId)
    {
        return await _context.Items.Where(i => i.CreatedByUserId == userId).Select(item => new ItemSummaryDto
        {
            ItemId = item.ItemId,
            ItemName = item.ItemName,
            TotalReviews = item.ItemReviews.Count(),
            AvgRating = item.ItemReviews.Any() ? (int)Math.Round(item.ItemReviews.Average(r => r.Rating)) : 0,
            CreatedBy = item.Creator.UserName,
            CreationDate = item.CreationDate,
        }).ToListAsync();
    }
}