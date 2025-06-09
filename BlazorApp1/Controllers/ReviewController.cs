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
    
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] ReviewViewModel review)
    {
        // var token = _httpContextAccessor.HttpContext.Request.Cookies["jwt"];
        // var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", review.jwtToken);
        var response = await _httpClient.PostAsJsonAsync("api/reviews", review);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<ReviewViewModel>();
            return Ok(content);
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorContent);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReviewById([FromQuery] int id)
    {
        var response = await _httpClient.GetAsync($"api/reviews/{id}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<ReviewViewModel>();
            return Ok(content);
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorContent);
        }
    }

    [HttpGet("{id}/comments")]
    public async Task<IActionResult> GetCommentsByReviewIdAsync([FromQuery] int reviewId)
    {
        var response = await _httpClient.GetAsync($"api/comment/reviews/{reviewId}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<Comment>();
            return Ok(content);
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorContent);
        }
    }
}