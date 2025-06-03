using BlazorApp1.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ReviewController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public ReviewController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ReviewAnythingAPI");
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetAllCategories()
    {
        var response = await _httpClient.GetAsync("api/categories");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<IEnumerable<Category>>();
            return Ok(content);
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorContent);
        }
    }
}