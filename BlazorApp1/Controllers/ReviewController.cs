using System.Net.Http.Headers;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ReviewController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReviewController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClientFactory.CreateClient("ReviewAnythingAPI");
        _httpContextAccessor = httpContextAccessor;
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
    
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] ReviewViewModel review)
    {
        // var token = _httpContextAccessor.HttpContext.Request.Cookies["jwt"];
        // _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.PostAsJsonAsync("api/reviews", review);
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
}