using BlazorApp1.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public UserController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ReviewAnythingAPI");
    }

    [HttpPost("information")]
    public async Task<IActionResult> GetUsersInformation([FromBody] List<int> userIds)
    {
        var response = await _httpClient.PostAsJsonAsync("api/user", userIds);
        if (response.IsSuccessStatusCode)
        {
            return Ok(response.Content.ReadAsStringAsync().Result);
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorContent);
        }
    }

    [HttpGet("{userId}/summary")]
    public async Task<IActionResult> GetUserSummary([FromRoute] int userId)
    {
        var response = await _httpClient.GetAsync($"api/user/{userId}/summary");
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode);
        }

        var result = await response.Content.ReadFromJsonAsync<UserSummary>();
        return Ok(result);
    }
}