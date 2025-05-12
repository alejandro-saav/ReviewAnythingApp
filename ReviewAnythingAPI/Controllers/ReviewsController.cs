using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewAnythingAPI.DTOs.ReviewDTOs;
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
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId)) return Unauthorized();

            var createdReview = await _reviewService.CreateReviewAsync(reviewCreateRequestDto, userId);
            return CreatedAtAction(nameof(GetReviewById), new { reviewId = createdReview.ReviewId }, createdReview);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument when creating review");
            return BadRequest(new {error = "invalid review data"});
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "conflict when creating review");
            return Conflict(new {error = "Review already exists"});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Unhandled exception occured in CreateReview endpoint");
            return StatusCode(StatusCodes.Status500InternalServerError, new {error = "Internal Server Error"});
        }
    }

    [Authorize]
    [HttpPut("{reviewId}")]
    public async Task<IActionResult> UpdateReviewAsync([FromBody] ReviewUpdateRequestDto reviewUpdateRequestDto, [FromRoute] int reviewId)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId)) return Unauthorized();
            var updatedReview = await _reviewService.UpdateReviewAsync(reviewUpdateRequestDto, userId, reviewId);
            return Ok(updatedReview);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument when updating review {reviewId}", reviewId);
            return BadRequest(new { error = "Invalid review data" });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Review {reviewId} not found", reviewId);
            return NotFound(new { error = "Review not found" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized attempt to update review {reviewId}", reviewId);
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error updating review {reviewId}", reviewId);
            return StatusCode(StatusCodes.Status500InternalServerError, new {error = "Internal Server Error"});

        }
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserReviews([FromRoute] int userId)
    {
        try
        {
            var reviews = await _reviewService.GetAllReviewsByUserIdAsync(userId);
            if (reviews == null || !reviews.Any()) return NotFound(new { message = $"No reviews found for user {userId}"});
            return Ok(reviews);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error retrieving reviews for user {userId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Internal server error"});
        }
    }

    [HttpGet("item/{itemId}")]
    public async Task<IActionResult> GetItemReviewsByItemId([FromRoute] int itemId)
    {
        try
        {
            var reviews = await _reviewService.GetAllReviewsByItemIdAsync(itemId);
            if (reviews == null || !reviews.Any())
                return NotFound(new { message = $"No reviews found for item {itemId}" });
            return Ok(reviews);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error retrieving reviews for item {itemId}", itemId);
            return StatusCode(StatusCodes.Status500InternalServerError, new
                    { error = "Internal server error" });
        }
    }

    [HttpGet("{reviewId}")]
    public async Task<IActionResult> GetReviewById([FromRoute] int reviewId)
    {
        try
        {
            var review = await _reviewService.GetReviewByIdAsync(reviewId);
            if (review == null) return NotFound(new { message = $"Review {reviewId} was not found" });
            return Ok(review);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error retrieving review {reviewId}", reviewId);
            return StatusCode(StatusCodes.Status500InternalServerError, new {error = "Internal server error"});
        }
    }
}