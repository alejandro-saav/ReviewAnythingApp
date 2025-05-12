using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.DTOs.ReviewDTOs;
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
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<ReviewResponseDto> CreateReviewAsync(ReviewCreateRequestDto reviewCreateRequestDto, int userId)
    {
        try
        {
            // using transaction to ensure data consistency
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
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
                    itemId = item.ItemId;
                }
                else
                {
                    var itemExists = await _itemRepository.GetByIdAsync(reviewCreateRequestDto.ItemId.Value);
                    if (itemExists == null)
                    {
                        throw new ArgumentException($"Item with ID {reviewCreateRequestDto.ItemId.Value} not found");
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
                var tagsList = string.IsNullOrWhiteSpace(reviewCreateRequestDto.Tags)
                    ? new List<string>()
                    : reviewCreateRequestDto.Tags.Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t))
                        .Distinct(StringComparer.OrdinalIgnoreCase).ToList();

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

                // Process tags and create review-tag associations in batch
                var reviewTagsToAdd = new List<ReviewTag>();
                foreach (var tag in tagsList)
                {
                    var existingTag = await _tagRepository.GetTagByNameAsync(tag);
                    int tagId;
                    if (existingTag == null)
                    {
                        // Create new tag
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

                return new ReviewResponseDto
                {
                    ReviewId = createdReview.ReviewId,
                    Title = createdReview.Title,
                    Content = createdReview.Content,
                    CreationDate = createdReview.CreationDate,
                    LastEditedDate = createdReview.LastEditDate,
                    Rating = createdReview.Rating,
                    ItemId = createdReview.ItemId,
                    Tags = reviewCreateRequestDto.Tags,
                    ReviewVotes = 0
                };
            }
            catch (Exception ex)
            {
                // Roll back the transaction if any operation fails
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            // Log the validation error
            _logger.LogError(ex, "Userid does not match with review userid: {Message}", ex.Message);
            throw;
        }
        catch (InvalidOperationException ex)
        {
            // Log business rule violations
            _logger.LogError(ex, "Business rule violation in CreateReviewAsync: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in CreateReviewAsync: {Message}", ex.Message);
            throw new ApplicationException("An error occurred while creating the review", ex);
        }
    }

    public async Task<ReviewResponseDto> UpdateReviewAsync(ReviewUpdateRequestDto reviewUpdateRequestDto,
        int userId, int reviewId)
    {
        try
        {
            // using transaction to ensure data consistency
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                // review exists
                var reviewExists = await _reviewRepository.GetByIdAsync(reviewId);
                if (reviewExists == null)
                {
                    throw new ArgumentException($"Review with ID {reviewId} not found");
                }

                // Does review match the userId
                if (userId != reviewExists.UserId)
                {
                    throw new ArgumentException(
                        $"Review with ID {reviewId} does not belong to user");
                }
                

                var newTags = string.IsNullOrWhiteSpace(reviewUpdateRequestDto.Tags)
                    ? new List<string>()
                    : reviewUpdateRequestDto.Tags.Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct(
                            StringComparer.OrdinalIgnoreCase).ToList();

                // Update review
                reviewExists.Title = reviewUpdateRequestDto.Title;
                reviewExists.Content = reviewUpdateRequestDto.Content;
                reviewExists.LastEditDate = DateTime.UtcNow;
                reviewExists.Rating = reviewUpdateRequestDto.Rating;
                
                // Process tags and create review-tag associations in batch
                var reviewTagsToAdd = new List<ReviewTag>();
                foreach (var tag in newTags)
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
                
                return new ReviewResponseDto
                {
                    ReviewId = reviewExists.ReviewId,
                    Title = reviewExists.Title,
                    Content = reviewExists.Content,
                    CreationDate = reviewExists.CreationDate,
                    LastEditedDate = reviewExists.LastEditDate,
                    Rating = reviewExists.Rating,
                    ItemId = reviewExists.ItemId,
                    Tags = reviewUpdateRequestDto.Tags,
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (ArgumentException ex)
        {
            // Log the validation error
            _logger.LogError(ex, "Validation error in UpdateReviewAsync: {Message}", ex.Message);
            throw;
        }
    }
    
    public async Task<IEnumerable<ReviewResponseDto>> GetAllReviewsByUserIdAsync(int userId)
    {
        var reviews = await _reviewRepository.GetAllReviewsByUserIdAsync(userId);
        var reviewDtos = new List<ReviewResponseDto>();

        foreach (var review in reviews)
        {
            reviewDtos.Add(new ReviewResponseDto
            {
                ReviewId = review.ReviewId,
                Title = review.Title,
                Content = review.Content,
                CreationDate = review.CreationDate,
                LastEditedDate = review.LastEditDate,
                Rating = review.Rating,
                ItemId = review.ItemId
            });
        };
        return reviewDtos;
    }

    public async Task<IEnumerable<ReviewResponseDto>> GetAllReviewsByItemIdAsync(int itemId)
    {
        var reviews = await _reviewRepository.GetAllReviewsByItemIdAsync(itemId);
        
        var reviewDtos = new List<ReviewResponseDto>();

        foreach (var review in reviews)
        {
            reviewDtos.Add(new ReviewResponseDto
            {
                ReviewId = review.ReviewId,
                Title = review.Title,
                Content = review.Content,
                CreationDate = review.CreationDate,
                LastEditedDate = review.LastEditDate,
                Rating = review.Rating,
                ItemId = review.ItemId
            });
        }

        return reviewDtos;
    }

    public async Task<bool> DeleteReviewAsync(int reviewId, int userId)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);
        if (review == null) return false;
        var isUserOwner = userId == review.UserId;
        if (!isUserOwner) return false;
        await _reviewRepository.DeleteAsync(reviewId);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<ReviewResponseDto> GetReviewByIdAsync(int reviewId)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);
        if (review == null)
        {
            throw new ArgumentException($"Review with ID {reviewId} not found");
        }

        return new ReviewResponseDto
        {
            ReviewId = review.ReviewId,
            Title = review.Title,
            Content = review.Content,
            CreationDate = review.CreationDate,
            LastEditedDate = review.LastEditDate,
            Rating = review.Rating,
            ItemId = review.ItemId
        };
    }
}