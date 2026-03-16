using Microsoft.AspNetCore.Mvc;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;
[ApiController]
[Route("api/[Controller]")]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;
    private readonly ILogger<ItemController> _logger;

    public ItemController(ILogger<ItemController> logger, IItemService itemService)
    {
        _logger = logger;
        _itemService = itemService;
    }

    [HttpGet("categories/{categoryId}")]
    public async Task<IActionResult> GetItemsByCategoryIdAsync(int categoryId)
    {
            var result = await _itemService.GetItemsByCategoryIdAsync(categoryId);

            _logger.LogInformation("Successfully retrieved items by category id. Category id: {CategoryId}, at: {Time}", categoryId, DateTime.UtcNow);

            return Ok(result);
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetItemsByUserIdAsync(int userId)
    {
            var result = await _itemService.GetItemsByUserIdAsync(userId);

            _logger.LogInformation("Successfully retrieved items by user id. User id: {UserId}, at: {Time}", userId, DateTime.UtcNow);

            return Ok(result);
    }

    [HttpGet("{itemName}")]
    public async Task<IActionResult> GetItemByNameAsync(string itemName)
    {
            if (string.IsNullOrWhiteSpace(itemName)) return BadRequest("Item name cannot be empty");
            var result = await _itemService.GetItemByNameAsync(itemName);
            if (result == null) return NotFound($"Item with name '{itemName}' not found");

            _logger.LogInformation("Successfully retrieved item by name. Item name: {ItemName}, at {Time}", itemName, DateTime.UtcNow);

            return Ok(result);
    }

    [HttpGet("/api/categories")]
    public async Task<IActionResult> GetAllCategoriesAsync()
    {
        var result = await _itemService.GetAllCategoriesAsync();
        return Ok(result);
    }
}