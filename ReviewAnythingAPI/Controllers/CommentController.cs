using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewAnythingAPI.DTOs.CommentDTOs;
using ReviewAnythingAPI.DTOs.ReviewDTOs;
using ReviewAnythingAPI.Enums;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;
[ApiController]
[Route("/api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }
    
    [HttpGet("{commentId}")]
    public async Task<IActionResult> GetCommentByIdAsync([FromRoute] int commentId)
    {
        var comment = await _commentService.GetCommentByIdAsync(commentId);
        return Ok(comment);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetAllCommentsByUser([FromRoute] int userId)
    {
            var comments = await _commentService.GetAllCommentsByUserAsync(userId);
            return Ok(comments);
    }

    [HttpGet("reviews/{reviewId}")]
    public async Task<IActionResult> GetAllCommentsByReview([FromRoute] int reviewId)
    {
            var comments = await _commentService.GetAllCommentsByReviewAsync(reviewId);
            return Ok(comments);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateCommentAsync([FromBody] CommentCreateRequestDto comment)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId)) return Unauthorized();
        if (userName == null) return Unauthorized();
        
        var createdComment = await _commentService.CreateCommentAsync(comment, userId, userName);
        return CreatedAtAction(nameof(GetCommentByIdAsync), new { commentId = createdComment.CommentId }, createdComment);
    }

    [Authorize]
    [HttpPut("{commentId}")]
    public async Task<IActionResult> UpdateCommentAsync([FromBody] CommentCreateRequestDto comment,
        [FromRoute] int commentId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId)) return Unauthorized();
        if (userName == null) return Unauthorized();

        var updatedComment = await _commentService.UpdateCommentAsync(comment, commentId, userId, userName);
        return Ok(updatedComment);
    }
    

    [Authorize]
    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteCommentByIdAsync([FromRoute] int commentId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId)) return Unauthorized();
        await _commentService.DeleteCommentByIdAsync(commentId, userId);
        return NoContent();
    }

    [Authorize]
    [HttpPost("comment-votes")]
    public async Task<IActionResult> PostCommentVotes([FromBody] CommentVoteRequestDto vote)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId)) return Unauthorized();
        var response = await _commentService.CommentVoteAsync(vote, userId);
        return response.ActionType switch
        {
            ActionType.Created => CreatedAtAction(nameof(PostCommentVotes), new { vote.CommentId }, vote),
            ActionType.Updated => Ok(vote),
            ActionType.Deleted => NoContent(),
            _ => StatusCode(500, "Internal server error")
        };
    }
}