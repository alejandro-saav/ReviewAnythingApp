using BlazorApp1.Models;
using BlazorApp1.Models.Auth;

namespace BlazorApp1.Services;

public interface IUserService
{
    string? LastErrorMessage { get; }
    Task<IEnumerable<UserCommentInformation?>> GetUsersInformationAsync(IEnumerable<int> userIds);
}