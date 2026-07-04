using System.Collections;
using System.Security.Claims;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewAnythingAPI.DTOs.AuthDTOs;
using ReviewAnythingAPI.DTOs.FollowDTOs;
using ReviewAnythingAPI.DTOs.UserDTOs;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;

[ApiController]
[Produces("application/json")]
[ApiVersion("1.0")]
[Route("api/[Controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IFollowService _followService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, IFollowService followService, ILogger<UserController> logger)
    {
        _userService = userService;
        _followService = followService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a lists of user information for the given list of user ids.
    /// </summary>
    /// <returns>A list of users information with details including: User id, username, profile image, review count and followers count.</returns>
    /// <response code="200">Returns a list of users information.</response>
    /// <response code="409">Invalid user ids or no valid user ids found.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpPost("information")]
    [ProducesResponseType(typeof(UserCommentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsersCommentInformationAsync([FromBody] IReadOnlyList<int> userIds)
    {
        var usersInformation = await _userService.GetUsersCommentInformationAsync(userIds);

        _logger.LogInformation("Retrieved user's information by comment. At: {Time}", DateTime.UtcNow);

        return Ok(usersInformation);
    }


    /// <summary>
    /// Retrieves the summary details of a user by the current authenticated user.
    /// </summary>
    /// <remarks>The endpoint is restricted to authenticated users only.</remarks>
    /// <returns>A user summary with details including: User id, username, profile image, firstname, lastname and bio.</returns>
    /// <response code="200">Request successful, returns the user summary details.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [Authorize]
    [HttpGet("summary")]
    [ProducesResponseType(typeof(UserSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserSummaryAsync()
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId)) return Unauthorized();
        var user = await _userService.GetUserSummaryAsync(userId);

        _logger.LogInformation("Retrieved user summary. User id: {UserId}, at: {Time}", userId, DateTime.UtcNow);

        return Ok(user);
    }

    /// <summary>
    /// Updates the summary details of a user by the current authenticated user.
    /// </summary>
    /// <remarks>The endpoint is restricted to authenticated users only.</remarks>
    /// <returns>The updated user summary with details including: User id, username, profile image, firstname, lastname and bio.</returns>
    /// <response code="200">Request successful, returns the updated user summary details.</response>
    /// <response code="400">Invalid user information or missing required fields.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [Authorize]
    [HttpPatch("summary")]
    [ProducesResponseType(typeof(UserSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUserSummaryAsync([FromForm] UserUpdateSummaryDto updateDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId)) return Unauthorized();
        var result = await _userService.UpdateUserSummaryAsync(userId, updateDto);

        _logger.LogInformation("Updated user summary. User id: {UserId}, at: {Time}", userId, DateTime.UtcNow);

        return Ok(result);
    }

    /// <summary>
    /// Follows the given user id by the current authenticated user.
    /// </summary>
    /// <remarks>The endpoint is restricted to authenticated users only.</remarks>
    /// <returns>Both user ids and the date of the follow.</returns>
    /// <response code="200">Request successful, returns the follower and followee user ids and the date of the follow.</response>
    /// <response code="400">User id must be positive number.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [Authorize]
    [HttpPost("{targetUserId}/follow")]
    [ProducesResponseType(typeof(FollowResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> FollowUser([FromRoute] int targetUserId)
    {
        if (targetUserId <= 0) return BadRequest("User id must be a positive number.");
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId)) return Unauthorized();
        var result = await _followService.FollowUserAsync(userId, targetUserId);

        _logger.LogInformation("New user follow. Actor id: {ActorUserId}, target user id: {TargetUserId}, at: {Time}", userId, targetUserId, DateTime.UtcNow);

        return Ok(result);
    }

    /// <summary>
    /// Unfollows the given user id by the current authenticated user.
    /// </summary>
    /// <remarks>The endpoint is restricted to authenticated users only.</remarks>
    /// <returns>Both user ids and the date of the follow.</returns>
    /// <response code="200">Request successful, returns the follower and followee user ids and the date of the follow.</response>
    /// <response code="400">User id must be positive number.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [Authorize]
    [HttpDelete("{targetUserId}/follow")]
    [ProducesResponseType(typeof(FollowResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UnfollowUser([FromRoute] int targetUserId)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId)) return Unauthorized();
        var success = await _followService.UnFollowUserAsync(userId, targetUserId);
        if (!success) return BadRequest();

        _logger.LogInformation("New user unfollow. Actor id: {ActorUserId}, target user id: {TargetUserId}, at: {Time}", userId, targetUserId, DateTime.UtcNow);

        return NoContent();
    }

    /// <summary>
    /// Retrieves a user extra detail information.
    /// </summary>
    /// <returns>A user with extra details including: User id, username, profile image, firstname, lastname, bio, number of total reviews, number of total comments, a list of followers, a list of followings and a bool indicating if the current authenticated user is following.</returns>
    /// <response code="200">Request successful, returns the user extra details.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpGet("{targetUserId}/page-data")]
    [ProducesResponseType(typeof(UserPageDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserProfilePageData([FromRoute] int targetUserId)
    {
        int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int currentUserId);

        var success = await _userService.GetUserPageDataAsync(targetUserId, currentUserId);

        _logger.LogInformation("Retrieved user profile page data. User id: {UserId}, at: {Time}", targetUserId, DateTime.UtcNow);

        return Ok(success);
    }

    /// <summary>
    /// Retrieves a given amount of user ids, order by creation date descending.
    /// </summary>
    /// <returns>A given amount of user ids.</returns>
    /// <response code="200">Request successful, returns a list of user ids.</response>
    /// <response code="500">Internal server error. Please try again.</response>
    [HttpGet("latest")]
    [ProducesResponseType(typeof(IEnumerable<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenericResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLatestUserIdsAsync([FromQuery] int amount)
    {
        var userIds = await _userService.GetLatestUserIdsAsync(amount);
        return Ok(userIds);
    }
}