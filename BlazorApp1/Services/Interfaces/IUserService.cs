using BlazorApp1.Models;

namespace BlazorApp1.Services;

public interface IUserService
{
    string? LastErrorMessage { get; }
    Task<IEnumerable<UserCommentDto?>> GetUsersInformationAsync(IEnumerable<int> userIds);

    Task<UserSummary?> GetUserSummaryAsync();
    Task<bool> UnFollowUserAsync(FollowRequest followRequest);
    Task<FollowResponse?> FollowUserAsync(FollowRequest followRequest);

    Task<UserPageData?> GetUserPageDataAsync(int userId);
    Task<UserSummary?> UpdateUserSummaryAsync(UserSummaryModel model);
}