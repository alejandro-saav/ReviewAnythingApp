using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.DTOs.ReviewDTOs;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;

namespace ReviewAnythingAPI.Repositories;

public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(ReviewAnythingDbContext context) : base(context)
    {}

    public async Task<IEnumerable<Review>> GetAllReviewsByUserIdAsync(int userId)
    {
        return await _context.Reviews.Where(r => r.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetAllReviewsByItemIdAsync(int itemId)
    {
        return await _context.Reviews.Where(r => r.ItemId == itemId).ToListAsync();
    }

    public async Task<Review> GetReviewByUserIdAndItemIdAsync(int userId, int itemId)
    {
        return await _context.Reviews.FirstOrDefaultAsync(r => r.UserId == userId && r.ItemId == itemId);
    }

    public async Task<ReviewDetailDto?> GetReviewDetailByIdAsync(int reviewId)
    {
        var review =  await _context.Reviews.Where(r => r.ReviewId == reviewId).Select(r => new ReviewDetailDto
        {
            ReviewId = r.ReviewId,
            Title = r.Title,
            Content = r.Content,
            CreationDate = r.CreationDate,
            LastEditDate = r.LastEditDate,
            Rating = r.Rating,
            ItemId = r.ItemId,
            UserName = r.Creator != null ? r.Creator.UserName ?? "" : "",
            UserId = r.UserId,
            Tags = r.ReviewTags.Select(rt => rt.Tag.TagName).ToList(),
            UpVoteCount = r.ReviewVotes.Count(rv => rv.VoteType == 1),
            DownVoteCount = r.ReviewVotes.Count(rv => rv.VoteType == -1),
            TotalVotes = r.ReviewVotes.Count()
        }).FirstOrDefaultAsync();
        return review;
    }
}