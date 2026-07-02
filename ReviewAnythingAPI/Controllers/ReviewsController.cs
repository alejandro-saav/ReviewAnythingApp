using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewAnythingAPI.DTOs.AuthDTOs;
using ReviewAnythingAPI.DTOs.ReviewDTOs;
using ReviewAnythingAPI.Enums;
using ReviewAnythingAPI.HelperClasses;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;

[ApiController]
[Produces("application/json")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
    {
        _reviewService = reviewService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new review resource.
    /// </summary>
    /// <remarks>This endpoint is restricted to authenticated users only.</remarks>
    /// <returns>The newly created review and its details including: Review id, title, content, creation date, last edit date, rating, user id, item id, tags, up vote count, down vote count and total votes.</returns>
    /// <response code="201">Review successfully created, returns the newly created review.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="409">Conflict: A review for this user already exists.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ReviewSummaryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateReviewAsync([FromBody] ReviewCreateRequestDto reviewCreateRequestDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId)) return Unauthorized();

        var createdReview = await _reviewService.CreateReviewAsync(reviewCreateRequestDto, userId);

        _logger.LogInformation("Review successfully created. User id: {UserId}, review id: {ReviewId}, at {Time}", userId, createdReview.ReviewId, DateTime.UtcNow);

        return CreatedAtAction(nameof(GetReviewById), new { reviewId = createdReview.ReviewId }, createdReview);
    }


    /// <summary>
    /// Updates a review resource by its unique identifier.
    /// </summary>
    /// <remarks>The endpoint is restricted to authenticated users only.</remarks>
    /// <param name="reviewId">The unique identifier of a review.</param>
    /// <param name="reviewUpdateRequestDto">The new review object with the fields require to update.</param>
    /// <returns>The updated review with its details.</returns>
    /// <response code="200">Review successfully updated returns the review details.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not own the review.</response>
    /// <response code="404">No review was found by the given movie id.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [Authorize]
    [HttpPut("{reviewId}")]
    [ProducesResponseType(typeof(ReviewSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateReviewAsync([FromBody] ReviewUpdateRequestDto reviewUpdateRequestDto, [FromRoute] int reviewId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId)) return Unauthorized();
        var updatedReview = await _reviewService.UpdateReviewAsync(reviewUpdateRequestDto, userId, reviewId);

        _logger.LogInformation("Successfully review update. User id_ {UserId}, review id {ReviewId}, at: {Time}", userId, reviewId, DateTime.UtcNow);

        return Ok(updatedReview);
    }

    /// <summary>
    /// Retrieves all reviews made by the given user unique identifier.
    /// </summary>
    /// <param name="userId">The user unique identifier (int)</param>
    /// <returns>A list of reviews with details including: Review id, title, content, creation date, last edit date, rating, user information, item id, tags, up vote count, down vote count and total votes.</returns>
    /// <response code="200">Returns a list of reviews or an empty array if no reviews found for the given userId.</response>
    /// <response code="400">Invalid userId.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpGet("users/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<ReviewDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserReviews([FromRoute] int userId)
    {
        if (userId <= 0) return BadRequest("User id must be a positive integer.");
        var reviews = await _reviewService.GetAllReviewsByUserIdAsync(userId);

        _logger.LogInformation("Retrieved user reviews. User id: {UserId}, at: {Time}", userId, DateTime.UtcNow);

        return Ok(reviews);
    }

    /// <summary>
    /// Retrieves all reviews made by the given item unique identifier.
    /// </summary>
    /// <param name="itemId">The item unique identifier (int)</param>
    /// <returns>A list of reviews with details including: Review id, title, content, creation date, last edit date, rating, user information, item id, tags, up vote count, down vote count and total votes.</returns>
    /// <response code="200">Returns a list of reviews or an empty array if no reviews found for the given itemId.</response>
    /// <response code="400">Invalid itemId.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpGet("item/{itemId}")]
    [ProducesResponseType(typeof(IEnumerable<ReviewDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetItemReviewsByItemId([FromRoute] int itemId)
    {
        if (itemId <= 0) return BadRequest("Item id must be a positive integer.");
        var reviews = await _reviewService.GetAllReviewsByItemIdAsync(itemId);

        _logger.LogInformation("Retrieved reviews by item id. Item id: {itemId}, reviews count_ {ReviewsCount}, at {Time}", itemId, reviews.Count(), DateTime.UtcNow);

        return Ok(reviews);
    }

    /// <summary>
    /// Retrieves the details of a review by its unique identifier.
    /// </summary>
    /// <param name="reviewId">The unique identifier of a review (int).</param>
    /// <returns>A review with details including: Review id, title, content, creation date, last edit date, rating, user information, item id, tags, up vote count, down vote count and total votes.</returns>
    /// <response code="200">Review found, returns the review details.</response>
    /// <response code="404">A review was not found for the given review id.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpGet("{reviewId}")]
    [ProducesResponseType(typeof(ReviewDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetReviewById([FromRoute] int reviewId)
    {
        var review = await _reviewService.GetReviewByIdAsync(reviewId);

        _logger.LogInformation("Retrieved review by id. Review id: {ReviewId}, at: {Time}", reviewId, DateTime.UtcNow);

        return Ok(review);
    }

    /// <summary>
    /// Retrieves a list of the latest created reviews ids.
    /// </summary>
    /// <param name="amount">The amount of reviews ids you want to get if not specified the default is 250 (int).</param>
    /// <returns>A list of the latest created reviews ids.</returns>
    /// <response code="200">A list of the latest created reviews ids.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpGet("latest")]
    [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLatestReviewsIds([FromQuery] int? amount)
    {
        if (amount is null || amount <= 0) amount = 250;
        var reviewsIds = await _reviewService.GetLatestReviewsIdsAsync(amount.Value);
        return Ok(reviewsIds);
    }


    /// <summary>
    /// Add if not exists, update if exists and is different or delete if it is the same review vote.
    /// </summary>
    /// <remarks>The endpoint is restricted to authenticated users only.</remarks>
    /// <response code="200">Review vote successfully updated.</response>
    /// <response code="201">Review vote successfully created.</response>
    /// <response code="204">Review vote successfully deleted.</response>
    /// <response code="400">Missing parameter fields or invalid vote type. Vote type allow values 1 or -1.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">A Review was not found for the given review id.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [Authorize]
    [HttpPost("review-votes")]
    [ProducesResponseType(typeof(ReviewVoteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ReviewVoteResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Retrieves a review details with its comments, votes and detailed information about the current user comment votes and user follows.
    /// </summary>
    /// <param name="reviewId">The unique identifier of a review (int).</param>
    /// <returns>A review details including: Review id, title, content, creation date, last edit date, rating, user information, item id, tags, comments, up vote count, down vote count and total votes. And current user details for the review including: comments votes, user follows and user review vote.</returns>
    /// <response code="200">Review found, returns the review with extra details.</response>
    /// <response code="404">A review was not found for the given review id.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpGet("{reviewId}/page-data")]
    [ProducesResponseType(typeof(ReviewPageDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetReviewPageData([FromRoute] int reviewId)
    {
        int? userId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int id))
            {
                userId = id;
            }
        }

        var data = await _reviewService.GetReviewPageDataAsync(userId, reviewId);

        _logger.LogInformation("Retrieved review page data. Review id: {ReviewId}, at: {Time}", reviewId, DateTime.UtcNow);

        return Ok(data);
    }

    /// <summary>
    /// Retrieves all reviews made by the current authenticated user.
    /// </summary>
    /// <remarks>The endpoint is restricted to authenticated users only.</remarks>
    /// <param name="queryParamsDto">Filter options including: Page, rating, category, tags, sort, search.</param>
    /// <returns>A list of reviews with details including: Review id, category, title, content, last edit date, rating, user information, number of comments, creator followers, total reviews, tags, up vote count, down vote count and total votes.</returns>
    /// <response code="200">Returns a list of reviews or an empty array if no reviews found for the current authenticated user.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [Authorize]
    [HttpGet("myreviews")]
    [ProducesResponseType(typeof(IEnumerable<LikesReviewsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetMyReviewsAsync([FromQuery] ExploreQueryParamsDto queryParamsDto)
    {
        int pageSize = 9;
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId)) return Unauthorized();
        var reviews = await _reviewService.GetMyReviewsAsync(userId, pageSize, queryParamsDto);

        _logger.LogInformation("Retrieved reviews made by authenticated user. User id: {UserId}, at: {Time}", userId, DateTime.UtcNow);

        return Ok(reviews);
    }

    /// <summary>
    /// Retrieves all liked reviews made by the current authenticated user.
    /// </summary>
    /// <remarks>The endpoint is restricted to authenticated users only.</remarks>
    /// <param name="queryParamsDto">Filter options including: Page, rating, category, tags, sort, search.</param>
    /// <returns>A list of reviews with details including: Review id, category, title, content, last edit date, rating, user information, number of comments, creator followers, total reviews, tags, up vote count, down vote count and total votes.</returns>
    /// <response code="200">Returns a list of reviews or an empty array if no liked reviews found for the current authenticated user.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [Authorize]
    [HttpGet("liked-reviews")]
    [ProducesResponseType(typeof(IEnumerable<LikesReviewsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLikedReviewsAsync([FromQuery] ExploreQueryParamsDto queryParamsDto)
    {
        int pageSize = 9;
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId)) return Unauthorized();
        var reviews = await _reviewService.GetLikesReviewsAsync(userId, pageSize, queryParamsDto);

        _logger.LogInformation("Retrieved liked reviews by authenticated user. User id: {UserId}, at: {Time}", userId, DateTime.UtcNow);

        return Ok(reviews);
    }

    /// <summary>
    /// Retrieves a list of the last created.
    /// </summary>
    /// <param name="queryParamsDto">Filter options including: Page, rating, category, tags, sort, search.</param>
    /// <returns>A list of reviews with details including: Review id, category, title, content, last edit date, rating, user information, number of comments, creator followers, total reviews, tags, up vote count, down vote count and total votes.</returns>
    /// <response code="200">Returns a list of detail reviews.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpGet("explore")]
    [ProducesResponseType(typeof(IEnumerable<LikesReviewsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetExplorePageReviewsAsync([FromQuery] ExploreQueryParamsDto queryParamsDto)
    {
        int pageSize = 9;
        var reviews = await _reviewService.GetExplorePageReviewsAsync(queryParamsDto, pageSize);

        _logger.LogInformation("Retrieved explore page information. At: {Time}", DateTime.UtcNow);

        return Ok(reviews);
    }
}