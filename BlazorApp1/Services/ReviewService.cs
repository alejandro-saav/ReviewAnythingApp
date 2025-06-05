using BlazorApp1.Models;

namespace BlazorApp1.Services;

public class ReviewService : IReviewService
{
    private readonly HttpClient _httpClient;
    public string? LastErrorMessage { get; private set; }

    public ReviewService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("BlazorAppApi");
    }

    public async Task<IEnumerable<Category>> GetAllReviewCategoriesAsync()
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.GetAsync("api/review/categories");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<IEnumerable<Category>>(); 
                return content ?? [];
            } else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                LastErrorMessage = $"Error fetching categories, status code: {response.StatusCode} - {errorContent}, error message: {errorContent}";
                return [];
            }
        }
        catch (Exception ex)
        {
            LastErrorMessage = "Something went wrong";
            return [];
        }
    }

    public async Task<object> CreateReviewAsync(ReviewViewModel review)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/review", review);
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadFromJsonAsync<object>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                LastErrorMessage = $"Error creating review, status code: {response.StatusCode} - error message:{errorContent}";
                return new { };
            }
        }
        catch (Exception ex)
        {
            LastErrorMessage = "Something went wrong";
            return new {};
        }
    }
}