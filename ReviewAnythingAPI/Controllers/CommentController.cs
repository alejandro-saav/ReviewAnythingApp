using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewAnythingAPI.DTOs.AuthDTOs;
using ReviewAnythingAPI.DTOs.CommentDTOs;
using ReviewAnythingAPI.DTOs.ReviewDTOs;
using ReviewAnythingAPI.Enums;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;

[ApiController]
[Produces("application/json")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentController> _logger;

    public CommentController(ICommentService commentService, ILogger<CommentController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }


    /// <summary>
    /// Retrieves all comments by the user unique identifier.
    /// </summary>
    /// <param name="userId">The user unique identifier (int)</param>
    /// <returns>A list of comments with details including: CommentId, Content, ReviewId, LastEditDate, UserInformation and number of likes</returns>
    /// <response code="200">Returns a list of comments or an empty array if no comments found for the given userId.</response>
    /// <response code="400">Invalid userId.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<CommentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllCommentsByUser([FromRoute] int userId)
    {
        var comments = await _commentService.GetAllCommentsByUserAsync(userId);

        _logger.LogInformation("Successfully retrieved all comments by user. User id: {UserId}, at: {Time}", userId, DateTime.UtcNow);

        return Ok(comments);
    }

    /// <summary>
    /// Retrieves all comments from a review unique identifier.
    /// </summary>
    /// <param name="reviewId">The review unique identifier (int)</param>
    /// <returns>A list of comments with details including: CommentId, Content, ReviewId, LastEditDate, UserInformation and number of likes.</returns>
    /// <response code="200">Returns a list of comments or an empty array if no comments found for the given reviewId.</response>
    /// <response code="400">Invalid reviewId.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpGet("reviews/{reviewId}")]
    [ProducesResponseType(typeof(IEnumerable<CommentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllCommentsByReview([FromRoute] int reviewId)
    {
        var comments = await _commentService.GetAllCommentsByReviewAsync(reviewId);

        _logger.LogInformation("Successfully retrieve all comments by review id. Review id: {ReviewId}, at {Time}", reviewId, DateTime.UtcNow);

        return Ok(comments);
    }

    /// <summary>
    /// Creates a new comment resource.
    /// </summary>
    /// <remarks>This endpoint is restricted to authenticated users only.</remarks>
    /// <returns>The newly created comment and its details including: CommentId, Content, ReviewId, LastEditDate, UserInformation and number of Likes.</returns>
    /// <response code="201">Comment successfully created, returns the newly created comment.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Review not found for the given reviewId.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(IEnumerable<CommentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCommentAsync([FromBody] CommentCreateRequestDto comment)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId)) return Unauthorized();
        if (userName == null) return Unauthorized();

        var createdComment = await _commentService.CreateCommentAsync(comment, userId, userName);

        _logger.LogInformation("Comment created successfully. User id: {UserId}, username: {UserName}, comment id: {CommentId}, at: {Time} ", userId, userName, createdComment.CommentId, DateTime.UtcNow);

        return CreatedAtAction(nameof(GetCommentById), new { commentId = createdComment.CommentId }, createdComment);
    }

    /// <summary>
    /// Retrieves the details of a comment by its unique identifier.
    /// </summary>
    /// <param name="commentId">The unique identifier of a comment (int).</param>
    /// <returns>A comment with details including: CommentId, Content, ReviewId, LastEditDate, UserInformation and number of likes</returns>
    /// <response code="200">Comment found, returns the comment details.</response>
    /// <response code="404">A comment was not found for the given comment id.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpGet("{commentId}")]
    [ProducesResponseType(typeof(IEnumerable<CommentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCommentById([FromRoute] int commentId)
    {
        var comment = await _commentService.GetCommentByIdAsync(commentId);

        _logger.LogInformation("Successfully retrieve comment. Comment id: {CommentId}, at: {Time}", commentId, DateTime.UtcNow);

        return Ok(comment);
    }

    /// <summary>
    /// Update a comment resource by its unique identifier.
    /// </summary>
    /// <remarks>The endpoint is restricted to authenticated users only.</remarks>
    /// <param name="commentId">The unique identifier of a comment (int).</param>
    /// <param name="comment">A comment details including: Content and ReviewId.</param>
    /// <returns>A comment with details including: CommentId, Content, ReviewId, LastEditDate, UserInformation and number of likes</returns>
    /// <response code="200">Comment successfully updated, returns the comment details.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not allowed to perform the operation.</response>
    /// <response code="404">A comment was not found for the given comment id.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [Authorize]
    [HttpPut("{commentId}")]
    [ProducesResponseType(typeof(IEnumerable<CommentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<CommentResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(IEnumerable<CommentResponseDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCommentAsync([FromBody] CommentCreateRequestDto comment,
        [FromRoute] int commentId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId)) return Unauthorized();
        if (userName == null) return Unauthorized();

        var updatedComment = await _commentService.UpdateCommentAsync(comment, commentId, userId, userName);

        _logger.LogInformation("Succefully updated comment. User id: {UserId}, username: {UserName}, comment id {CommentId}, at {Time}", userId, userName, updatedComment.CommentId, DateTime.UtcNow);

        return Ok(updatedComment);
    }


    /// <summary>
    /// Deletes a comment resource by its unique identifier.
    /// </summary>
    /// <remarks>The endpoint is restricted to authenticated users only.</remarks>
    /// <param name="commentId">The unique identifier of a comment (int).</param>
    /// <response code="204">Comment successfully deleted.</response>
    /// <response code="400">Invalid comment id.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not allowed to perform the operation.</response>
    /// <response code="404">A comment was not found for the given comment id.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [Authorize]
    [HttpDelete("{commentId}")]
    [ProducesResponseType(typeof(IEnumerable<CommentResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(IEnumerable<CommentResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<CommentResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(IEnumerable<CommentResponseDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteCommentByIdAsync([FromRoute] int commentId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId)) return Unauthorized();
        await _commentService.DeleteCommentByIdAsync(commentId, userId);

        _logger.LogInformation("Successfully delete comment. Comment id: {CommentId}, user id: {UserId}, at {Time}", commentId, userId, DateTime.UtcNow);

        return NoContent();
    }

    /// <summary>
    /// Add if not exists, update if exists and is different or delete if it is the same comment vote.
    /// </summary>
    /// <remarks>The endpoint is restricted to authenticated users only.</remarks>
    /// <response code="200">Comment vote successfully updated.</response>
    /// <response code="201">Comment vote successfully created.</response>
    /// <response code="204">Comment vote successfully deleted.</response>
    /// <response code="400">Missing parameter fields or invalid vote type. Vote type allow values 1 or -1.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">A comment was not found for the given comment id.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [Authorize]
    [HttpPost("comment-votes")]
    [ProducesResponseType(typeof(IEnumerable<CommentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<CommentResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(IEnumerable<CommentResponseDto>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<CommentResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IEnumerable<CommentResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostCommentVotes([FromBody] CommentVoteRequestDto vote)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId)) return Unauthorized();
        var response = await _commentService.CommentVoteAsync(vote, userId);

        return response.ActionType switch
        {
            ActionType.Created => CreatedAtAction(nameof(PostCommentVotes), new { vote.CommentId }, vote),
            ActionType.Updated => Ok(vote),
            ActionType.Deleted => NoContent(),
            _ => StatusCode(500, "Internal server error")
        };
    }

    /// <summary>
    /// Retrieves all comments by the user unique identifier with more details about.
    /// </summary>
    /// <remarks>The endpoint is restricted to authenticated users only.</remarks>
    /// <returns>A list of comments with extra details including: CommentId, Content, ReviewId, LastEditDate, Likes, ReviewTitle, UserName and user profile image.</returns>
    /// <response code="200">Returns a list of comments or an empty array if no comments found for the authenticated user.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [Authorize]
    [HttpGet("mycomments-page")]
    public async Task<IActionResult> GetMyCommentsPage()
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId)) return Unauthorized();
        var comments = await _commentService.GetAllCommentsPageAsync(userId);

        _logger.LogInformation("Successfully retrieve all comments made by the authenticated user. User id: {UserId}, at: {Time}", userId, DateTime.UtcNow);

        return Ok(comments);
    }
}