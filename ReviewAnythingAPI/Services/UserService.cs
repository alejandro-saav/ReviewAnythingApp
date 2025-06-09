using ReviewAnythingAPI.DTOs.UserDTOs;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserCommentDto>> GetUsersCommentInformationAsync(IReadOnlyList<int> userIds)
    {
        if (userIds == null || userIds.Count == 0)
        {
            return [];
        }
        var usersInfo = await _userRepository.GetUsersCommentInformationAsync(userIds);
        return usersInfo;
    }
}