using ReviewAnythingAPI.DTOs.UserDTOs;
using ReviewAnythingAPI.HelperClasses.CustomExceptions;
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

    public async Task<UserSummaryDto> GetUserSummaryAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new EntityNotFoundException("User not found");
        return new UserSummaryDto
        {
            UserId = user.Id,
            UserName = user.UserName,
            ProfileImage = user.ProfileImage,
            FirstName = user.FirstName,
        };
    }

    public async Task<bool> UpdateUserAsync(int userId, UserUpdateRequestDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new EntityNotFoundException("User not found");
        if (updateDto.FirstName != null) user.FirstName = updateDto.FirstName;
        if (updateDto.LastName != null) user.LastName = updateDto.LastName;
        if (updateDto.ProfileImage != null) user.PhoneNumber = updateDto.PhoneNumber;
        if (updateDto.ProfileImage != null) user.ProfileImage = updateDto.ProfileImage;
        if (updateDto.Bio != null) user.ProfileImage = updateDto.Bio;
        return true;
    }

    public async Task<UserPageDataDto> GetUserPageDataAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if(user == null) throw new EntityNotFoundException("User not found");
        var userPageDate = await _userRepository.GetUserPageDataAsync(userId);
        return userPageDate;
    }
}