using ReviewAnythingAPI.DTOs.UserDTOs;
using ReviewAnythingAPI.HelperClasses.CustomExceptions;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly CloudinaryService _cloudinaryService;

    public UserService(IUserRepository userRepository, CloudinaryService cloudinaryService)
    {
        _userRepository = userRepository;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<IEnumerable<UserCommentDto>> GetUsersCommentInformationAsync(IReadOnlyList<int> userIds)
    {
        if (userIds is null || userIds.Count == 0)
        {
            throw new InvalidOperationException("No user ids found.");
        }
        var usersInfo = await _userRepository.GetUsersCommentInformationAsync(userIds);
        return usersInfo;
    }

    public async Task<UserSummaryDto> GetUserSummaryAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null) throw new EntityNotFoundException("User not found");
        return new UserSummaryDto
        {
            UserId = user.Id,
            UserName = user.UserName ?? "",
            ProfileImage = user.ProfileImage ?? "",
            FirstName = user.FirstName ?? "",
            LastName = user.LastName ?? "",
            Bio = user.Bio ?? "",
        };
    }


    // METHOD NOT BEING USED
    public async Task<bool> UpdateUserAsync(int userId, UserUpdateRequestDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null) throw new EntityNotFoundException("User not found");
        if (updateDto.FirstName is not null) user.FirstName = updateDto.FirstName;
        if (updateDto.LastName is not null) user.LastName = updateDto.LastName;
        if (updateDto.ProfileImage is not null) user.PhoneNumber = updateDto.PhoneNumber;
        if (updateDto.ProfileImage is not null) user.ProfileImage = updateDto.ProfileImage;
        if (updateDto.Bio is not null) user.ProfileImage = updateDto.Bio;
        return true;
    }

    public async Task<UserPageDataDto> GetUserPageDataAsync(int targetUserId, int currentUserId)
    {
        var user = await _userRepository.GetByIdAsync(targetUserId);
        if(user is null) throw new EntityNotFoundException("User not found");
        var userPageDate = await _userRepository.GetUserPageDataAsync(targetUserId, currentUserId);
        return userPageDate;
    }

    public async Task<UserSummaryDto> UpdateUserSummaryAsync(int userId, UserUpdateSummaryDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null) throw new EntityNotFoundException("User not found");
        var summary = new UserSummaryDto
        {
            FirstName = updateDto.FirstName,
            LastName = updateDto.LastName ?? "",
            Bio = updateDto.Bio ?? "",
        };
        if (updateDto.ProfileImage is not null && updateDto.ProfileImage?.Length > 0)
        {
            try
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(updateDto.ProfileImage, user.Id);
                user.ProfileImage = imageUrl;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error while uploading the image to cloudinary on updating user summary.Error:{ex.Message}");
            }
        } else if (updateDto.ProfileImage is null && updateDto.DeleteImage)
        {
            user.ProfileImage = null;
        }
        await _userRepository.UpdateUserSummaryAsync(user,  summary);
        return summary;
    }
}