using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using ReviewAnythingAPI.DTOs.AuthDTOs;
using ReviewAnythingAPI.DTOs.ItemDTOs;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;

[ApiController]
[Produces("application/json")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}[Controller]")]
public class ItemController : ControllerBase
{
    private readonly IItemService _itemService;
    private readonly ILogger<ItemController> _logger;

    public ItemController(ILogger<ItemController> logger, IItemService itemService)
    {
        _logger = logger;
        _itemService = itemService;
    }

    /// <summary>
    /// Retrieves all items by the category unique identifier.
    /// </summary>
    /// <param name="categoryId">The category unique identifier (int).</param>
    /// <returns>A list of items with details including: item id, item name, total reviews, created by, avg rating and creation date.</returns>
    /// <response code="200">Returns a list of items or an empty array if no items found for the given category id.</response>
    /// <response code="400">Invalid category id.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpGet("categories/{categoryId}")]
    [ProducesResponseType(typeof(IEnumerable<ItemSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetItemsByCategoryIdAsync(int categoryId)
    {
        var result = await _itemService.GetItemsByCategoryIdAsync(categoryId);

        _logger.LogInformation("Successfully retrieved items by category id. Category id: {CategoryId}, at: {Time}", categoryId, DateTime.UtcNow);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves all items by the user unique identifier.
    /// </summary>
    /// <param name="userId">The user unique identifier (int).</param>
    /// <returns>A list of items with details including: item id, item name, total reviews, created by, avg rating and creation date.</returns>
    /// <response code="200">Returns a list of items or an empty array if no items found for the given user id.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpGet("users/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<ItemSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetItemsByUserIdAsync(int userId)
    {
        var result = await _itemService.GetItemsByUserIdAsync(userId);

        _logger.LogInformation("Successfully retrieved items by user id. User id: {UserId}, at: {Time}", userId, DateTime.UtcNow);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves the item resource by the item name.
    /// </summary>
    /// <param name="itemName">The item name (string).</param>
    /// <returns>An item with details including: item id, item name, total reviews, created by, avg rating and creation date.</returns>
    /// <response code="200">Returns an item resource for the given item name.</response>
    /// <response code="400">Invalid item name.</response>
    /// <response code="404">No item found for the given item name.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpGet("{itemName}")]
    [ProducesResponseType(typeof(IEnumerable<ItemSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<ItemSummaryDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetItemByNameAsync(string itemName)
    {
        if (string.IsNullOrWhiteSpace(itemName)) return BadRequest("Item name cannot be empty");
        var result = await _itemService.GetItemByNameAsync(itemName);
        if (result == null) return NotFound($"Item with name '{itemName}' not found");

        _logger.LogInformation("Successfully retrieved item by name. Item name: {ItemName}, at {Time}", itemName, DateTime.UtcNow);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a list of categories available for a review.
    /// </summary>
    /// <returns>Retrieves a list of categories available for a review.</returns>
    /// <response code="200">Returns a list of categories.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpGet("/api/categories")]
    [ProducesResponseType(typeof(IEnumerable<ItemSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllCategoriesAsync()
    {
        var result = await _itemService.GetAllCategoriesAsync();
        return Ok(result);
    }
}