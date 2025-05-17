using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Repositories;

public class TagRepository : Repository<Tag>, ITagRepository
{
    public TagRepository(ReviewAnythingDbContext context) : base(context) {}

    public async Task<Tag?> GetTagByNameAsync(string tagName)
    {
        return await _context.Tags.FirstOrDefaultAsync(t => t.TagName == tagName);
    }
}