using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;
[ApiController]
[Route("/api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentController> _logger;

    public CommentController(ICommentService commentService, ILogger<CommentController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    [Authorize]
    [HttpGet("user")]
    public async Task<IActionResult> GetAllCommentsByUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId)) return Unauthorized();
            var comments = await _commentService.GetAllCommentsByUserAsync(userId);
            if (!comments.Any()) return NotFound();
            return Ok(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("reviews/{reviewId}")]
    public async Task<IActionResult> GetAllCommentsByReview([FromRoute] int reviewId)
    {
        try
        {
            var comments = await _commentService.GetAllCommentsByReviewAsync(reviewId);
            if (!comments.Any()) return NotFound();
            return Ok(comments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            return StatusCode(500, "Internal server error");
        }
    }
}