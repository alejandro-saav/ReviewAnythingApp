using BlazorApp1.Models;
// using BlazorApp1.Services.Interfaces;
using Category = BlazorApp1.Models.Category;

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
            var response = await _httpClient.GetAsync("client/review/categories");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<IEnumerable<Category>>();
                return content ?? [];
            }
            else
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

    public async Task<ReviewModel> CreateReviewAsync(ReviewViewModel review)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.PostAsJsonAsync("client/review", review);
            if (response.IsSuccessStatusCode)
            {
                var newReview = await response.Content.ReadFromJsonAsync<ReviewModel>();
                return newReview;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                LastErrorMessage = $"Error creating review, status code: {response.StatusCode} - error message:{errorContent}";
                return new ReviewModel();
            }
        }
        catch (Exception ex)
        {
            LastErrorMessage = "Something went wrong";
            return new ReviewModel();
        }
    }

    public async Task<ReviewModel?> GetReviewByIdAsync(int reviewId)
    {
        Console.WriteLine("ENTER REVIEW SERVICE METHOD");
        Console.WriteLine($"HTTPCLIENT: {_httpClient.BaseAddress}");
        LastErrorMessage = null;
        try
        {
            Console.WriteLine("ENTER REVIEW SERVICE METHOD BEFORE RESPONSE");
            var response = await _httpClient.GetAsync($"client/review/{reviewId}");
            Console.WriteLine("ENTER REVIEW SERVICE METHOD AFTER RESPONSE");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("REVIEW SERVICE SUCCESS");
                var review = await response.Content.ReadFromJsonAsync<ReviewModel>();
                return review;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                LastErrorMessage = $"Error while fetching the review, status code: {response.StatusCode} - error message:{errorContent}";
                Console.WriteLine($"REVIEW SERVICE FAILURE: {errorContent}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"REVIEW SERVICE CATCH: {ex.Message}");
            LastErrorMessage = "Something went wrong";
            return null;
        }
    }

    public async Task<IEnumerable<Comment>> GetCommentsByReviewIdAsync(int reviewId)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.GetAsync($"client/review/{reviewId}/comments");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<IEnumerable<Comment>>();
                return content;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                LastErrorMessage = $"Error while fetching the review, status code: {response.StatusCode} - error message:{errorContent}";
                return [];
            }
        }
        catch (Exception ex)
        {
            LastErrorMessage = "Something went wrong";
            return [];
        }
    }

    public async Task<Comment?> CreateCommentAsync(CreateComment comment)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.PostAsJsonAsync("client/review/comment", comment);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<Comment>();
                return content;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                LastErrorMessage = $"Error while fetching the review, status code: {response.StatusCode} - error message:{errorContent}";
                return null;
            }
        }
        catch (Exception ex)
        {
            LastErrorMessage = $"Something went wrong ${ex.Message}";
            return null;
        }
    }
}