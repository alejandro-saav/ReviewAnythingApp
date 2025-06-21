using System.Net.Http.Headers;
using BlazorApp1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Controllers;

[ApiController]
[Route("client/[controller]")]

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
            var content = await response.Content.ReadFromJsonAsync<ReviewModel>();
            return Ok(content);
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorContent);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReviewById([FromRoute] int id)
    {
        Console.WriteLine("CONTROLLER CHECKPOINT");
        var response = await _httpClient.GetAsync($"api/reviews/{id}");
        Console.WriteLine("CONTROLLER AFTER RESOPONSE");
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("CONTROLLER SUCCESS");
            var content = await response.Content.ReadFromJsonAsync<ReviewModel>();
            return Ok(content);
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"CONTROLLER FAIL {errorContent}");
            return StatusCode((int)response.StatusCode, errorContent);
        }
    }

    [HttpGet("{reviewId}/comments")]
    public async Task<IActionResult> GetCommentsByReviewIdAsync([FromRoute] int reviewId)
    {
        var response = await _httpClient.GetAsync($"api/comment/reviews/{reviewId}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<IEnumerable<Comment>>();
            return Ok(content);
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorContent);
        }
    }

    [HttpPost("comment")]
    public async Task<IActionResult> CreateCommentAsync([FromBody] CreateComment comment)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", comment.jwtToken);
        var response = await _httpClient.PostAsJsonAsync("api/Comment", comment);
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

    [HttpPost("vote")]
    public async Task<IActionResult> ReviewVoteAsync([FromBody] ReviewVote vote)
    {
        _httpClient.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue("Bearer", vote.jwtToken);
        var response = await _httpClient.PostAsJsonAsync("api/reviews/review-votes", vote);
        if (response.IsSuccessStatusCode)
        {
            return Ok();
        }
        var errorContent = await response.Content.ReadAsStringAsync();
        return StatusCode((int)response.StatusCode, errorContent);
    }
}