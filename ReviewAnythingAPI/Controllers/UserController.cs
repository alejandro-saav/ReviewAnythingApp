using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewAnythingAPI.DTOs.UserDTOs;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IFollowService _followService;

    public UserController(IUserService userService, IFollowService followService)
    {
        _userService = userService;
        _followService = followService;
    }


    [HttpPost("information")]
    public async Task<IActionResult> GetUsersCommentInformationAsync([FromBody] IReadOnlyList<int> userIds)
    {
        var usersInformation = await _userService.GetUsersCommentInformationAsync(userIds);
        return Ok(usersInformation);
    }

    [Authorize]
    [HttpGet("summary")]
    public async Task<IActionResult> GetUserSummaryAsync()
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId)) return Unauthorized();
        var user = await _userService.GetUserSummaryAsync(userId);
        return Ok(user);
    }

    [Authorize]
    [HttpPatch("summary")]
    public async Task<IActionResult> UpdateUserSummaryAsync([FromForm] UserUpdateSummaryDto updateDto)
    {
        // if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId)) return Unauthorized();
        var result = await _userService.UpdateUserSummaryAsync(userId, updateDto);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("{targetUserId}/follow")]
    public async Task<IActionResult> FollowUser([FromRoute] int targetUserId)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId)) return Unauthorized();
        var result = await _followService.FollowUserAsync(userId, targetUserId);
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{targetUserId}/follow")]
    public async Task<IActionResult> UnfollowUser([FromRoute] int targetUserId)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId)) return Unauthorized();
        var success = await _followService.UnFollowUserAsync(userId, targetUserId);
        if (!success) return BadRequest();
        return NoContent();
    }

    [Authorize]
    [HttpPatch]
    public async Task<IActionResult> UpdateUserAsync([FromBody] UserUpdateRequestDto updateDto)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId)) return Unauthorized();
        var success = await _userService.UpdateUserAsync(userId, updateDto);
        if (!success) return BadRequest();
        return NoContent();
    }

    [HttpGet("{targetUserId}/page-data")]
    public async Task<IActionResult> GetUserProfilePageData([FromRoute] int targetUserId)
    {
        int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int currentUserId);

        var success = await _userService.GetUserPageDataAsync(targetUserId, currentUserId);
        return Ok(success);
    }
}