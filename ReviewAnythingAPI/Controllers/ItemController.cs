using Microsoft.AspNetCore.Mvc;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;
[ApiController]
[Route("api/[Controller]")]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;

    public ItemController(ILogger<ItemController> logger, IItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet("categories/{categoryId}")]
    public async Task<IActionResult> GetItemsByCategoryIdAsync(int categoryId)
    {
            if (categoryId < 1 || categoryId > 6)
            {
                return BadRequest("Category ID must be between 1 and 6");
            }

            var result = await _itemService.GetItemsByCategoryIdAsync(categoryId);
            return Ok(result);
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetItemsByUserIdAsync(int userId)
    {
            var result = await _itemService.GetItemsByUserIdAsync(userId);
            return Ok(result);
    }

    [HttpGet("{itemName}")]
    public async Task<IActionResult> GetItemByNameAsync(string itemName)
    {
            if (string.IsNullOrWhiteSpace(itemName)) return BadRequest("Item name cannot be empty");
            var result = await _itemService.GetItemByNameAsync(itemName);
            if (result == null) return NotFound($"Item with name '{itemName}' not found");
            return Ok(result);
    }
}