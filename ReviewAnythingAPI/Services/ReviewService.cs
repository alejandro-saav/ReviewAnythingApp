using System.Security.Claims;
using System.Transactions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.DTOs.ReviewDTOs;
using ReviewAnythingAPI.Enums;
using ReviewAnythingAPI.HelperClasses.CustomExceptions;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories;
using ReviewAnythingAPI.Repositories.Interfaces;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IItemRepository _itemRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IReviewTagRepository _reviewTagRepository;
    private readonly ReviewAnythingDbContext _dbContext;
    private readonly IReviewVoteRepository _reviewVoteRepository;

    public ReviewService(IReviewRepository reviewRepository, IItemRepository itemRepository, ITagRepository tagRepository, IReviewTagRepository reviewTagRepository, ReviewAnythingDbContext dbContext, IReviewVoteRepository reviewVoteRepository)
    {
        _reviewRepository = reviewRepository;
        _itemRepository = itemRepository;
        _tagRepository = tagRepository;
        _reviewTagRepository = reviewTagRepository;
        _dbContext = dbContext;
        _reviewVoteRepository = reviewVoteRepository;
    }

    public async Task<ReviewSummaryDto> CreateReviewAsync(ReviewCreateRequestDto reviewCreateRequestDto, int userId)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            // using transaction to ensure data consistency
            int itemId;
            if (reviewCreateRequestDto.ItemId == null)
            {
                var itemToInsert = new Item
                {
                    ItemName = reviewCreateRequestDto.Title,
                    CategoryId = reviewCreateRequestDto.CategoryId,
                    CreationDate = DateTime.UtcNow,
                    CreatedByUserId = userId,
                };
                var item = await _itemRepository.AddAsync(itemToInsert);
                await _dbContext.SaveChangesAsync();
                itemId = item.ItemId;
            }
            else
            {
                var itemExists = await _itemRepository.GetByIdAsync(reviewCreateRequestDto.ItemId.Value);
                if (itemExists == null)
                {
                    throw new EntityNotFoundException($"Item with ID {reviewCreateRequestDto.ItemId.Value} not found");
                }

                itemId = reviewCreateRequestDto.ItemId.Value;
            }

            // Check if the user already reviewed this item to prevent duplicates
            var existingReview = await _reviewRepository.GetReviewByUserIdAndItemIdAsync(userId, itemId);
            if (existingReview != null)
            {
                throw new InvalidOperationException("You have already reviewed this item");
            }

            // Process tags
            if (reviewCreateRequestDto.Tags.Count > 0)
            {
                reviewCreateRequestDto.Tags = reviewCreateRequestDto.Tags.Select(t => t.Trim())
                    .Where(t => !string.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            }

            // Create the review
            var review = new Review
            {
                Title = reviewCreateRequestDto.Title,
                Content = reviewCreateRequestDto.Content,
                CreationDate = DateTime.UtcNow,
                LastEditDate = DateTime.UtcNow,
                Rating = reviewCreateRequestDto.Rating,
                UserId = userId,
                ItemId = itemId,
            };
            var createdReview = await _reviewRepository.AddAsync(review);
            await _dbContext.SaveChangesAsync();

            // Process tags and create review-tag associations in batch
            var reviewTagsToAdd = new List<ReviewTag>();
            foreach (var tag in reviewCreateRequestDto.Tags)
            {
                var existingTag = await _tagRepository.GetTagByNameAsync(tag);
                int tagId;
                if (existingTag == null)
                {
                    // Create new tag
                    var newTag = await _tagRepository.AddAsync(new Tag { TagName = tag });
                    await _dbContext.SaveChangesAsync();
                    tagId = newTag.TagId;
                }
                else
                {
                    tagId = existingTag.TagId;
                }

                // Add to batch collection
                reviewTagsToAdd.Add(new ReviewTag
                {
                    TagId = tagId,
                    ReviewId = createdReview.ReviewId,
                });
            }

            // Add all review-tags associations in a single batch operation
            if (reviewTagsToAdd.Count > 0)
            {
                await _reviewTagRepository.AddRangeAsync(reviewTagsToAdd);
            }

            // Save Changes
            await _dbContext.SaveChangesAsync();

            // Commit transaction
            await transaction.CommitAsync();

            return new ReviewSummaryDto()
            {
                ReviewId = createdReview.ReviewId,
                Title = createdReview.Title,
                Content = createdReview.Content,
                CreationDate = createdReview.CreationDate,
                LastEditDate = createdReview.LastEditDate,
                Rating = createdReview.Rating,
                ItemId = createdReview.ItemId,
                Tags = reviewCreateRequestDto.Tags
            };
        }
        catch (Exception ex)
        {
            // Roll back the transaction if any operation fails
            await transaction.RollbackAsync();
            throw new TransactionFailedException("Review create transaction failed", ex);
        }
    }


    public async Task<ReviewSummaryDto> UpdateReviewAsync(ReviewUpdateRequestDto reviewUpdateRequestDto,
        int userId, int reviewId)
    {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            // using transaction to ensure data consistency
            try
            {
                // review exists
                var reviewExists = await _reviewRepository.GetByIdAsync(reviewId);
                if (reviewExists == null)
                {
                    throw new EntityNotFoundException($"Review with ID {reviewId} not found");
                }

                // Does review match the userId
                if (userId != reviewExists.UserId)
                {
                    throw new UnauthorizedAccessException(
                        $"Review with ID {reviewId} does not belong to user");
                }
                

                // Process tags
                if (reviewUpdateRequestDto.Tags.Count > 0)
                {
                    reviewUpdateRequestDto.Tags = reviewUpdateRequestDto.Tags.Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                }

                // Update review
                reviewExists.Title = reviewUpdateRequestDto.Title;
                reviewExists.Content = reviewUpdateRequestDto.Content;
                reviewExists.LastEditDate = DateTime.UtcNow;
                reviewExists.Rating = reviewUpdateRequestDto.Rating;
                
                // Process tags and create review-tag associations in batch
                var reviewTagsToAdd = new List<ReviewTag>();
                foreach (var tag in reviewUpdateRequestDto.Tags)
                {
                    var existingTag = await _tagRepository.GetTagByNameAsync(tag);
                    int tagId;
                    if (existingTag == null)
                    {
                        var newTag = await _tagRepository.AddAsync(new Tag { TagName = tag });
                        tagId = newTag.TagId;
                    }
                    else
                    {
                        tagId = existingTag.TagId;
                    }

                    // Add to batch collection
                    reviewTagsToAdd.Add(new ReviewTag
                    {
                        TagId = tagId,
                        ReviewId = reviewExists.ReviewId,
                    });
                }
            
                // Remove all tags
                await _reviewTagRepository.DeleteAllTagsByReviewIdAsync(reviewExists.ReviewId);
                // Insert all tags in ReviewTags
                await _reviewTagRepository.AddRangeAsync(reviewTagsToAdd);

                // Save changes
                await _dbContext.SaveChangesAsync();
                
                // Commit transaction
                await transaction.CommitAsync();
                
                return new ReviewSummaryDto
                {
                    ReviewId = reviewExists.ReviewId,
                    Title = reviewExists.Title,
                    Content = reviewExists.Content,
                    CreationDate = reviewExists.CreationDate,
                    LastEditDate = reviewExists.LastEditDate,
                    Rating = reviewExists.Rating,
                    ItemId = reviewExists.ItemId,
                    Tags = reviewUpdateRequestDto.Tags,
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new TransactionFailedException("Review update transaction failed", ex);
            }
    }
    
    public async Task<IEnumerable<ReviewDetailDto>> GetAllReviewsByUserIdAsync(int userId)
    {
               var reviewDtos = await _dbContext.Reviews.Where(r => r.UserId == userId)
                   .Select(r => new ReviewDetailDto
                {
                    ReviewId = r.ReviewId,
                    Title = r.Title,
                    Content = r.Content,
                    CreationDate = r.CreationDate,
                    LastEditDate = r.LastEditDate,
                    Rating = r.Rating,
                    ItemId = r.ItemId,
                    UserId = userId,
                    Tags = r.ReviewTags.Select(rt => rt.Tag.TagName).ToList(),
                    DownVoteCount = r.ReviewVotes.Count(rv => rv.VoteType == -1),
                    UpVoteCount = r.ReviewVotes.Count(rv => rv.VoteType == 1),
                    TotalVotes = r.ReviewVotes.Count(),
                }).ToListAsync();
            return reviewDtos;
    }

    public async Task<IEnumerable<ReviewDetailDto>> GetAllReviewsByItemIdAsync(int itemId)
    {
            var reviewDtos = await _dbContext.Reviews.Where(r => r.ItemId == itemId)
                .Select(r => new ReviewDetailDto
                {
                    ReviewId = r.ReviewId,
                    Title = r.Title,
                    Content = r.Content,
                    CreationDate = r.CreationDate,
                    LastEditDate = r.LastEditDate,
                    Rating = r.Rating,
                    ItemId = r.ItemId,
                    UserId = r.UserId,
                    Tags = r.ReviewTags.Select(rt => rt.Tag.TagName).ToList(),
                    DownVoteCount = r.ReviewVotes.Count(rv => rv.VoteType == -1),
                    UpVoteCount = r.ReviewVotes.Count(rv => rv.VoteType == 1),
                    TotalVotes = r.ReviewVotes.Count(),
                }).ToListAsync();
            return reviewDtos;
    }

    public async Task<bool> DeleteReviewAsync(int reviewId, int userId)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);
        if (review == null) return false;
        var isUserOwner = userId == review.UserId;
        if (!isUserOwner) return false;
        await _reviewRepository.DeleteAsyncById(reviewId);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<ReviewDetailDto?> GetReviewByIdAsync(int reviewId)
    {
        var review =  await _dbContext.Reviews.Where(r => r.ReviewId == reviewId).Select(r => new ReviewDetailDto
        {
            ReviewId = r.ReviewId,
            Title = r.Title,
            Content = r.Content,
            CreationDate = r.CreationDate,
            LastEditDate = r.LastEditDate,
            Rating = r.Rating,
            ItemId = r.ItemId,
            UserId = r.UserId,
            Tags = r.ReviewTags.Select(rt => rt.Tag.TagName).ToList(),
            UpVoteCount = r.ReviewVotes.Count(rv => rv.VoteType == 1),
            DownVoteCount = r.ReviewVotes.Count(rv => rv.VoteType == -1),
            TotalVotes = r.ReviewVotes.Count()
        }).FirstOrDefaultAsync();
        return review != null ? review : null;
    }

    public async Task<ReviewVoteResponseDto> ReviewVoteAsync(ReviewVoteRequestDto reviewVoteRequestDto, int userId)
    {
        var existingVote = await _reviewVoteRepository.GetByUserAndReviewIdAsync(userId, reviewVoteRequestDto.ReviewId);
        var response = new ReviewVoteResponseDto
        {
            ReviewId = reviewVoteRequestDto.ReviewId,
            UserVote = reviewVoteRequestDto.VoteType
        };
        if (existingVote == null)
        {
            ReviewVote newReviewVote = new ReviewVote
            {
                ReviewId = reviewVoteRequestDto.ReviewId,
                UserId = userId,
                VoteType = reviewVoteRequestDto.VoteType,
                VoteDate = DateTime.UtcNow,
            };
            await _reviewVoteRepository.AddAsync(newReviewVote);
            response.ActionType = ActionType.Created;
        } else if (existingVote.VoteType == reviewVoteRequestDto.VoteType)
        {
            await _reviewVoteRepository.DeleteAsyncByEntity(existingVote);
            response.ActionType = ActionType.Deleted;
        }
        else
        {
            existingVote.VoteType = reviewVoteRequestDto.VoteType;
            existingVote.VoteDate = DateTime.UtcNow;
            response.ActionType = ActionType.Updated;
        }

        await _dbContext.SaveChangesAsync();
        return response;
    }
}