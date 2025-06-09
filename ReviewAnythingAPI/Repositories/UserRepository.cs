using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.DTOs.UserDTOs;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Repositories;

public class UserRepository : Repository<ApplicationUser>, IUserRepository
{
    public UserRepository(ReviewAnythingDbContext context) : base(context) {}

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
}