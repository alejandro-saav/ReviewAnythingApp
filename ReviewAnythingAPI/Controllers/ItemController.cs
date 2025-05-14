using Microsoft.AspNetCore.Mvc;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;
[ApiController]
[Route("api/[Controller]")]
public class ItemController : ControllerBase
{
    private readonly ILogger<ItemController> _logger;
    private readonly IItemService _itemService;

    public ItemController(ILogger<ItemController> logger, IItemService itemService)
    {
        _logger = logger;
        _itemService = itemService;
    }

    [HttpGet("categories/{categoryId}")]
    public async Task<IActionResult> GetItemsByCategoryIdAsync(int categoryId)
    {
        try
        {
            if (categoryId < 1 || categoryId > 6)
            {
                return BadRequest("Category ID must be between 1 and 6");
            }

            var result = await _itemService.GetItemsByCategoryIdAsync(categoryId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception.");
            return StatusCode(500, "An error occurred while retrieving items.");
        }
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetItemsByUserIdAsync(int userId)
    {
        try
        {
            var result = await _itemService.GetItemsByUserIdAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception.");
            return StatusCode(500, "An error occurred while retrieving items.");
        }
    }

    [HttpGet("{itemName}")]
    public async Task<IActionResult> GetItemByNameAsync(string itemName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(itemName)) return BadRequest("Item name cannot be empty");
            var result = await _itemService.GetItemByNameAsync(itemName);
            if (result == null) return NotFound($"Item with name '{itemName}' not found");
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception.");
            return StatusCode(500, "An error occurred while retrieving the item.");
        }
    }
}