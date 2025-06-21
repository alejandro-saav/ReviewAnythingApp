using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewAnythingAPI.DTOs.ReviewDTOs;
using ReviewAnythingAPI.Enums;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
    {
        _reviewService = reviewService;
        _logger = logger;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateReviewAsync([FromBody] ReviewCreateRequestDto reviewCreateRequestDto)
    {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId)) return Unauthorized();

            var createdReview = await _reviewService.CreateReviewAsync(reviewCreateRequestDto, userId);
            return CreatedAtAction(nameof(GetReviewById), new { reviewId = createdReview.ReviewId }, createdReview);
    }

    [Authorize]
    [HttpPut("{reviewId}")]
    public async Task<IActionResult> UpdateReviewAsync([FromBody] ReviewUpdateRequestDto reviewUpdateRequestDto, [FromRoute] int reviewId)
    {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId)) return Unauthorized();
            var updatedReview = await _reviewService.UpdateReviewAsync(reviewUpdateRequestDto, userId, reviewId);
            return Ok(updatedReview);
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserReviews([FromRoute] int userId)
    {
            var reviews = await _reviewService.GetAllReviewsByUserIdAsync(userId);
            if (reviews == null || !reviews.Any()) return NotFound(new { message = $"No reviews found for user {userId}"});
            return Ok(reviews);
    }

    [HttpGet("item/{itemId}")]
    public async Task<IActionResult> GetItemReviewsByItemId([FromRoute] int itemId)
    {
            var reviews = await _reviewService.GetAllReviewsByItemIdAsync(itemId);
            return Ok(reviews);
    }

    [HttpGet("{reviewId}")]
    public async Task<IActionResult> GetReviewById([FromRoute] int reviewId)
    {
            var review = await _reviewService.GetReviewByIdAsync(reviewId);
            if (review == null) return NotFound(new { message = $"Review {reviewId} was not found" });
            return Ok(review);
    }

    [Authorize]
    [HttpPost("review-votes")]
    public async Task<IActionResult> PostReviewVotes([FromBody] ReviewVoteRequestDto reviewVoteRequestDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId)) return Unauthorized();
        var reviewVote = await _reviewService.ReviewVoteAsync(reviewVoteRequestDto, userId);
        return reviewVote.ActionType switch
        {
            ActionType.Created => CreatedAtAction(nameof(PostReviewVotes), new { reviewVoteRequestDto.ReviewId },
                reviewVote),
            ActionType.Updated => Ok(reviewVote),
            ActionType.Deleted => NoContent(),
            _ => StatusCode(500, "Internal server error")
        };
    }

    [HttpGet("{reviewId}/page-data")]
    public async Task<IActionResult> GetReviewPageData([FromRoute] int reviewId)
    {
        int? userId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
        
        var data = await _reviewService.GetReviewPageDataAsync(userId, reviewId);
        return Ok(data);
    }
}