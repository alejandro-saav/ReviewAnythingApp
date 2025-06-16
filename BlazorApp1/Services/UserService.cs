using BlazorApp1.Models;
using BlazorApp1.Models.Auth;

namespace BlazorApp1.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;
    public string? LastErrorMessage { get; private set; }

    public UserService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("BlazorAppApi");
    }

    public async Task<IEnumerable<UserCommentDto?>> GetUsersInformationAsync(IEnumerable<int> userIds)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.PostAsJsonAsync("user", userIds);
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadFromJsonAsync<IEnumerable<UserCommentDto>>().Result;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                LastErrorMessage = $"Error creating review, status code: {response.StatusCode} - error message:{errorContent}";
                return null;
            }
        }
        catch (Exception ex)
        {
            LastErrorMessage = ex.Message;
            return null;
        }
    }

    public async Task<UserSummary?> GetUserSummaryAsync(int userId)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.GetAsync($"user/{userId}/summary");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserSummary>();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                LastErrorMessage = $"Error fetching user summary, status code: {response.StatusCode} - error message:{errorContent}";
                return null;
            }
        }
        catch (Exception ex)
        {
            LastErrorMessage = $"catch exception: {ex.Message}";
            return null;
        }
    }
}