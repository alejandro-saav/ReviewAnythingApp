using ReviewAnythingAPI.DTOs.FollowDTOs;
using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Services.Interfaces;

public interface IFollowService
{
    public Task<FollowResponseDto> FollowUserAsync(int userId, int targetUserId);
    public Task<bool> UnFollowUserAsync(int userId, int targetUserId);
}