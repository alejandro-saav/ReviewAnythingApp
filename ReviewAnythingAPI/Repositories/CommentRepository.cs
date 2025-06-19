using System.Runtime.InteropServices.Marshalling;
using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.DTOs.CommentDTOs;
using ReviewAnythingAPI.DTOs.UserDTOs;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Repositories;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(ReviewAnythingDbContext context) : base(context)
    { }

    public async Task<IEnumerable<CommentResponseDto>> GetAllCommentsByReviewIdAsync(int reviewId)
    {
        return await _context.Comments.Where(c => c.ReviewId == reviewId).Select(c => new CommentResponseDto
        {
            CommentId = c.CommentId,
            Content = c.Content,
            LastEditDate = c.LastEditDate,
            ReviewId = c.ReviewId,
            Likes = _context.CommentVotes.Where(cv => cv.CommentId == c.CommentId && cv.VoteType == 1).Count(),
            UserInformation = new UserCommentDto
            {
                UserId = c.UserId ?? 0,
                UserName = c.User.UserName,
                ProfileImage = c.User.ProfileImage,
                ReviewCount = _context.Reviews.Where(r => r.UserId == c.UserId).Count(),
                FollowerCount = _context.Follows.Where(f => f.FollowingUserId == c.UserId).Count(),
            }
        }).ToListAsync();
    }

    public async Task<IEnumerable<CommentResponseDto>> GetAllCommentsByUserIdAsync(int userId)
    {
        return await _context.Comments.Where(c => c.UserId == userId).Select(c => new CommentResponseDto
        {
            CommentId = c.CommentId,
            Content = c.Content,
            ReviewId = c.ReviewId,
            LastEditDate = c.LastEditDate,
            Likes = _context.CommentVotes.Where(cv => cv.CommentId == c.CommentId && cv.VoteType == 1).Count(),
            UserInformation = new UserCommentDto
            {
                UserId = userId,
                UserName = c.User.UserName,
                ProfileImage = c.User.ProfileImage,
                ReviewCount = _context.Reviews.Where(r => r.UserId == c.UserId).Count(),
                FollowerCount = _context.Follows.Where(f => f.FollowingUserId == c.UserId).Count(),
            }
        }).ToListAsync();
    }

    public async Task<CommentResponseDto> CreateCommentAsync(Comment comment)
    {
        var newComment = await _context.AddAsync(comment);
        await _context.SaveChangesAsync();
        return new CommentResponseDto
        {
            CommentId = comment.CommentId,
            Content = comment.Content,
            ReviewId = comment.ReviewId,
            LastEditDate = comment.LastEditDate,
            UserInformation = new UserCommentDto
            {
                UserId = comment.UserId,
                UserName = comment.User.UserName,
                ProfileImage = comment.User.ProfileImage,
                ReviewCount = _context.Reviews.Where(r => r.UserId == comment.UserId).Count(),
                FollowerCount = _context.Follows.Where(f => f.FollowingUserId == comment.UserId).Count(),
            },
            Likes = _context.CommentVotes.Where(cm => cm.CommentId == comment.CommentId && cm.VoteType == 1).Count(),
        };
    }
}