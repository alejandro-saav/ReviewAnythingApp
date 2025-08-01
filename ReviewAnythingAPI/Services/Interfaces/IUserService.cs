using ReviewAnythingAPI.DTOs.UserDTOs;
using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserCommentDto>> GetUsersCommentInformationAsync(IReadOnlyList<int> userIds);

    Task<UserSummaryDto> GetUserSummaryAsync(int userId);

    Task<bool> UpdateUserAsync(int userId, UserUpdateRequestDto updateDto);
    Task<UserPageDataDto> GetUserPageDataAsync(int targetUserId, int currentUserId);
    Task<UserSummaryDto> UpdateUserSummaryAsync(int userId, UserUpdateSummaryDto updateDto);
}