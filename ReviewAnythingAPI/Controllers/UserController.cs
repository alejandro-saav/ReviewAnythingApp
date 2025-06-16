using Microsoft.AspNetCore.Mvc;
using ReviewAnythingAPI.DTOs.UserDTOs;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService) => _userService = userService;


    [HttpPost("information")]
    public async Task<IActionResult> GetUsersCommentInformationAsync([FromBody] IReadOnlyList<int> userIds)
    {
        var usersInformation = await _userService.GetUsersCommentInformationAsync(userIds);
        return Ok(usersInformation);
    }

    [HttpGet("{userId}/summary")]
    public async Task<IActionResult> GetUserSummaryAsync([FromRoute] int userId)
    {
        var user = await _userService.GetUserSummaryAsync(userId);
        return Ok(user);
    }
}