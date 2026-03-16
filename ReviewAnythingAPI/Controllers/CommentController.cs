using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewAnythingAPI.DTOs.CommentDTOs;
using ReviewAnythingAPI.DTOs.ReviewDTOs;
using ReviewAnythingAPI.Enums;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentController> _logger;

    public CommentController(ICommentService commentService, ILogger<CommentController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }


    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetAllCommentsByUser([FromRoute] int userId)
    {
        var comments = await _commentService.GetAllCommentsByUserAsync(userId);

        _logger.LogInformation("Successfully retrieved all comments by user. User id: {UserId}, at: {Time}", userId, DateTime.UtcNow);

        return Ok(comments);
    }

    [HttpGet("reviews/{reviewId}")]
    public async Task<IActionResult> GetAllCommentsByReview([FromRoute] int reviewId)
    {
        var comments = await _commentService.GetAllCommentsByReviewAsync(reviewId);

        _logger.LogInformation("Successfully retrieve all comments by review id. Review id: {ReviewId}, at {Time}", reviewId, DateTime.UtcNow);

        return Ok(comments);
    }

    [Authorize]
    [HttpPost]
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

    [HttpGet("{commentId}")]
    public async Task<IActionResult> GetCommentById([FromRoute] int commentId)
    {
        var comment = await _commentService.GetCommentByIdAsync(commentId);

        _logger.LogInformation("Successfully retrieve comment. Comment id: {CommentId}, at: {Time}", commentId, DateTime.UtcNow);

        return Ok(comment);
    }

    [Authorize]
    [HttpPut("{commentId}")]
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


    [Authorize]
    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteCommentByIdAsync([FromRoute] int commentId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId)) return Unauthorized();
        await _commentService.DeleteCommentByIdAsync(commentId, userId);

        _logger.LogInformation("Successfully delete comment. Comment id: {CommentId}, user id: {UserId}, at {Time}", commentId, userId, DateTime.UtcNow);

        return NoContent();
    }

    [Authorize]
    [HttpPost("comment-votes")]
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