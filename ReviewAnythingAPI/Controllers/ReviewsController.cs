using Microsoft.AspNetCore.Mvc;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;
[ApiController]
[Route("[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserReviewsByUserId([FromRoute] int userId)
    {
        var reviews = _reviewService.GetAllReviewsByUserIdAsync(userId);
        return Ok(reviews);
    }

    [HttpGet("item/{itemId}")]
    public async Task<IActionResult> GetItemReviewsByItemId([FromRoute] int itemId)
    {
        var reviews = _reviewService.GetAllReviewsByItemIdAsync(itemId);
        return Ok(reviews);
    }
}