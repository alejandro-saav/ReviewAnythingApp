using System.Security.Claims;
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

    public ReviewService(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<ReviewResponseDto> CreateReviewAsync(ReviewCreateRequestDto reviewCreateRequestDto, int userId)
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
            itemId = reviewCreateRequestDto.ItemId.Value;
        }
        var tagsList = string.IsNullOrWhiteSpace(reviewCreateRequestDto.Tags)
            ? new List<string>()
            : reviewCreateRequestDto.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .ToList();
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
        
        foreach (var tag in tagsList)
        {
            if (await _tagRepository.GetTagByNameAsync(tag) == null)
            {
               var tagInserted = await _tagRepository.AddAsync(new Tag { TagName = tag });
               await _reviewTagRepository.AddAsync(new ReviewTag
                   { TagId = tagInserted.TagId, ReviewId = createdReview.ReviewId});
            }
        }

        return new ReviewResponseDto
        {
            ReviewId = createdReview.ReviewId,
            Title = createdReview.Title,
            Content = createdReview.Content,
            CreationDate = createdReview.CreationDate,
            LastEditedDate = createdReview.LastEditDate,
            Rating = createdReview.Rating,
            ItemId = createdReview.ItemId,
        };

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
}