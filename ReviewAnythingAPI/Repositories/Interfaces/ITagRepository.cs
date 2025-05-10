using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Repositories.Interfaces;

public interface ITagRepository : IRepository<Tag>
{
    public Task<Tag> GetTagByNameAsync(string tagName);
}