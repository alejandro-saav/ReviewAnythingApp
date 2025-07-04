using Microsoft.Identity.Client;
using ReviewAnythingAPI.DTOs.UserDTOs;
using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Repositories.Interfaces;

public interface IUserRepository : IRepository<ApplicationUser>
{
    public Task<IEnumerable<UserCommentDto>> GetUsersCommentInformationAsync(IReadOnlyList<int> userIds);
    public Task<UserPageDataDto> GetUserPageDataAsync(int targetUserId, int currentUserId);
    public Task<UserSummaryDto> UpdateUserSummaryAsync(ApplicationUser user, UserSummaryDto summary);
}