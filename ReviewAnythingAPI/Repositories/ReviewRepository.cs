using Microsoft.EntityFrameworkCore;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.DTOs.ReviewDTOs;
using ReviewAnythingAPI.DTOs.UserDTOs;
using ReviewAnythingAPI.HelperClasses;
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

    public async Task<IEnumerable<MyReviewsDto>> GetMyReviewsAsync(int userId)
    {
        var myReviews = await _context.Reviews.Where(r => r.UserId == userId).Select(r => new  MyReviewsDto
        {
            ReviewId = r.ReviewId,
            Title = r.Title,
            Content = r.Content,
            LastEditDate = r.LastEditDate,
            Rating = r.Rating,
            Likes = r.ReviewVotes.Where(rv => rv.VoteType == 1).Count(),
            NumberOfComments = r.ReviewComments.Count(),
            Tags = r.ReviewTags.Select(rt => rt.Tag.TagName).ToList(),
        }).ToListAsync();
        return myReviews;
    }

    public async Task<IEnumerable<LikesReviewsDto>> GetLikesReviewsAsync(int userId)
    {
        var reviews = await _context.ReviewVotes.Where(rv => rv.UserId == userId).Select(rv => new LikesReviewsDto
        {
            ReviewId = rv.ReviewId,
            Title = rv.Review.Title,
            Content = rv.Review.Content,
            LastEditDate = rv.Review.LastEditDate,
            Rating = rv.Review.Rating,
            Likes = rv.Review.ReviewVotes.Where(rv => rv.VoteType == 1).Count(),
            NumberOfComments = rv.Review.ReviewComments.Count(),
            Tags = rv.Review.ReviewTags.Select(rr => rr.Tag.TagName).ToList(),
            User = new UserSummaryDto
            {
                UserId = rv.Review.UserId ?? 0,
                UserName = rv.Review.Creator.UserName ?? "",
                ProfileImage = rv.Review.Creator.ProfileImage ?? "",
            },
            CreatorFollowers = _context.Follows.Where(f => f.FollowingUserId == rv.Review.UserId).Count()
        }).ToListAsync();
        return reviews;
    }

    public async Task<IEnumerable<LikesReviewsDto>> GetExplorePageReviewsAsync(ExploreQueryParamsDto queryParamsDto, int pageSize)
    {
        var query = _context.Reviews.AsQueryable();
        if (queryParamsDto.Sort == null || !SortOptions.All.Contains(queryParamsDto.Sort))
        {
            queryParamsDto.Sort = SortOptions.DateDesc;
        }

        if (queryParamsDto.Rating is not null)
        {
            query = query.Where(r => r.Rating == queryParamsDto.Rating);
        }

        if (!string.IsNullOrEmpty(queryParamsDto.Category))
        {
            query = query.Where(r => r.ReviewItem.ItemCategory.CategoryName == queryParamsDto.Category);
        }

        if (queryParamsDto.Tags.Any())
        {
                query = query.Where(r => queryParamsDto.Tags.All(t => r.ReviewTags.Select(rt => rt.Tag.TagName).Contains(t)));
        }

        switch (queryParamsDto.Sort?.ToLower())
        {
            case SortOptions.RatingAsc:
                query = query.OrderBy(r => r.Rating);
                break;
            case SortOptions.RatingDesc:
                query = query.OrderByDescending(r => r.Rating);
                break;
            case SortOptions.DateAsc:
                query = query.OrderBy(r => r.CreationDate);
                break;
            case SortOptions.DateDesc:
                query = query.OrderByDescending(r => r.CreationDate);
                break;
            default:
                query = query.OrderByDescending(r => r.CreationDate);
                break;
        }
        int total = await _context.Reviews.CountAsync();
        var reviews = await query.Skip((queryParamsDto.Page - 1) * pageSize).Take(pageSize).Select(r => new LikesReviewsDto
        {
            ReviewId = r.ReviewId,
            Category = r.ReviewItem.ItemCategory.CategoryName,
            Title = r.Title,
            Content = r.Content,
            Likes = r.ReviewVotes.Where(rv => rv.VoteType == 1).Count(),
            LastEditDate = r.LastEditDate,
            Rating = r.Rating,
            Tags = r.ReviewTags.Select(rt => rt.Tag.TagName).ToList(),
            NumberOfComments = r.ReviewComments.Count(),
            User = new  UserSummaryDto
            {
                UserId = r.UserId ?? 0,
                UserName = r.Creator.UserName ?? "",
                ProfileImage = r.Creator.ProfileImage ?? "",
            },
            CreatorFollowers = r.Creator.UserFollows.Count(),
            PageNumber = queryParamsDto.Page,
            PageSize = pageSize,
            Total = total,
        }).ToListAsync();
        return reviews;
    }
}