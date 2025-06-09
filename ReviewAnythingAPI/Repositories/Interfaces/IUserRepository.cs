using Microsoft.Identity.Client;
using ReviewAnythingAPI.DTOs.UserDTOs;
using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Repositories.Interfaces;

public interface IUserRepository : IRepository<ApplicationUser>
{
    public Task<IEnumerable<UserCommentDto>> GetUsersCommentInformationAsync(IReadOnlyList<int> userIds);
}