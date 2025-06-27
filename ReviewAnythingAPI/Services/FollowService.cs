using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.DTOs.FollowDTOs;
using ReviewAnythingAPI.HelperClasses.CustomExceptions;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories;
using ReviewAnythingAPI.Repositories.Interfaces;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Services;

public class FollowService : IFollowService
{
    private readonly IFollowRepository _followRepository;
    private readonly IUserRepository _userRepository;
    private readonly ReviewAnythingDbContext _dbContext;

    public FollowService(IFollowRepository followRepository, IUserRepository userRepository, ReviewAnythingDbContext dbContext)
    {
        _followRepository = followRepository;
        _userRepository = userRepository;
        _dbContext = dbContext;
    }

    public async Task<FollowResponseDto> FollowUserAsync(int userId, int targetUserId)
    {
        var targetExist = await _userRepository.GetByIdAsync(targetUserId);
        if (targetExist == null)
        {
            throw new EntityNotFoundException("User not found");
        }

        var follow = new Follow
        {
            FollowerUserId = userId,
            FollowingUserId = targetUserId,
            FollowDate = DateTime.UtcNow
        };
        await _followRepository.AddAsync(follow);
        await _dbContext.SaveChangesAsync();
        return new FollowResponseDto
        {
            UserId = userId,
            TargetUserId = targetUserId,
            FollowDate = follow.FollowDate
        };
    }

    public async Task<bool> UnFollowUserAsync(int userId, int targetUserId)
    {
        var targetExist = await _userRepository.GetByIdAsync(targetUserId);
        if (targetExist == null)
        {
            throw new EntityNotFoundException("User not found");
        }

        var entity = await _followRepository.GetEntityByIdAsync(userId, targetUserId);
        if (entity == null)
        {
            throw new EntityNotFoundException("User not found");
        }

        await _followRepository.DeleteAsyncByEntity(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}