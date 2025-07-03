using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.DTOs.UserDTOs;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Repositories;

public class UserRepository : Repository<ApplicationUser>, IUserRepository
{
    public UserRepository(ReviewAnythingDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserCommentDto>> GetUsersCommentInformationAsync(IReadOnlyList<int> userIds)
    {
        var usersInfo = await _context.Users.Where(u => userIds.Contains(u.Id)).Select(u => new UserCommentDto
        {
            UserId = u.Id,
            UserName = u.UserName,
            ProfileImage = u.ProfileImage,
            ReviewCount = _context.Reviews.Where(r => r.UserId == u.Id).Count(),
            FollowerCount = _context.Follows.Where(f => f.FollowingUserId == u.Id).Count()
        }).ToListAsync();
        return usersInfo;
    }

    public async Task<UserPageDataDto> GetUserPageDataAsync(int targetUserId, int currentUserId)
    {
        var user = await _context.Users
            .Where(u => u.Id == targetUserId)
            .Select(u => new UserSummaryDto
            {
                UserId = u.Id,
                UserName = u.UserName ?? "",
                FirstName = u.FirstName ?? "",
                LastName = u.LastName ?? "",
                ProfileImage = u.ProfileImage,
                CreationDate = u.CreationDate,
                Bio = u.Bio ?? "",
            })
            .FirstOrDefaultAsync();

        var totalReviews = await _context.Reviews
            .Where(r => r.UserId == targetUserId)
            .CountAsync();

        var totalComments = await _context.Comments
            .Where(c => c.UserId == targetUserId)
            .CountAsync();

        var followers = await _context.Follows
            .Where(f => f.FollowingUserId == targetUserId)
            .Select(f => new UserSummaryDto
            {
                UserName = f.Follower!.UserName!,
                ProfileImage = f.Follower.ProfileImage,
            })
            .ToListAsync();

        var following = await _context.Follows
            .Where(f => f.FollowerUserId == targetUserId)
            .Select(f => new UserSummaryDto
            {
                UserName = f.Following!.UserName!,
                ProfileImage = f.Following.ProfileImage,
            })
            .ToListAsync();

        var IsUserFollowing = false;

        if (currentUserId > 0)
        {
            var CurrentUserFollowTarget = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerUserId == currentUserId && f.FollowingUserId == targetUserId);
            if (CurrentUserFollowTarget != null)
            {
                IsUserFollowing = true;
            }
        }

        return new UserPageDataDto
        {
            UserSummary = user,
            TotalReviews = totalReviews,
            TotalComments = totalComments,
            Followers = followers,
            Following = following,
            IsCurrentUserFollowing = IsUserFollowing,
        };
    }
}