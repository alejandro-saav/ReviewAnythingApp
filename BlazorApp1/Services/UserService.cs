using System.Net.Http.Headers;
using BlazorApp1.Models;
using BlazorApp1.Services;

namespace BlazorApp1.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;
    public string? LastErrorMessage { get; private set; }

    public UserService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ReviewAnythingAPI");
    }

    public async Task<IEnumerable<UserCommentDto?>> GetUsersInformationAsync(IEnumerable<int> userIds)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/user", userIds);
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
    // public async Task<IEnumerable<UserCommentDto?>> GetUsersInformationAsync(IEnumerable<int> userIds)
    // {
    //     LastErrorMessage = null;
    //     try
    //     {
    //         var response = await _httpClient.PostAsJsonAsync("user", userIds);
    //         if (response.IsSuccessStatusCode)
    //         {
    //             return response.Content.ReadFromJsonAsync<IEnumerable<UserCommentDto>>().Result;
    //         }
    //         else
    //         {
    //             var errorContent = await response.Content.ReadAsStringAsync();
    //             LastErrorMessage = $"Error creating review, status code: {response.StatusCode} - error message:{errorContent}";
    //             return null;
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         LastErrorMessage = ex.Message;
    //         return null;
    //     }
    // }

    public async Task<UserSummary?> GetUserSummaryAsync(int userId)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.GetAsync($"api/user/{userId}/summary");
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
    // public async Task<UserSummary?> GetUserSummaryAsync(int userId)
    // {
    //     LastErrorMessage = null;
    //     try
    //     {
    //         var response = await _httpClient.GetAsync($"user/{userId}/summary");
    //         if (response.IsSuccessStatusCode)
    //         {
    //             return await response.Content.ReadFromJsonAsync<UserSummary>();
    //         }
    //         else
    //         {
    //             var errorContent = await response.Content.ReadAsStringAsync();
    //             LastErrorMessage = $"Error fetching user summary, status code: {response.StatusCode} - error message:{errorContent}";
    //             return null;
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         LastErrorMessage = $"catch exception: {ex.Message}";
    //         return null;
    //     }
    // }

    public async Task<bool> UnFollowUserAsync(FollowRequest followRequest)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.DeleteAsync($"api/user/{followRequest.TargetUserId}/follow");
            if (response.IsSuccessStatusCode) return true;
            return false;
        }
        catch (Exception ex)
        {
            LastErrorMessage = $"catch exception: {ex.Message}";
            return false;
        }
    }

    public async Task<FollowResponse?> FollowUserAsync(FollowRequest followRequest)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.PostAsync($"api/user/{followRequest.TargetUserId}/follow", null);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<FollowResponse>();
                return content;
            }

            return null;
        }
        catch (Exception ex)
        {
            LastErrorMessage = $"catch exception: {ex.Message}";
            return null;
        }
    }

    public async Task<UserPageData?> GetUserPageDataAsync(int userId)
    {
        try
        {

            var response = await _httpClient.GetAsync($"api/user/{userId}/page-data");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<UserPageData>();
                return content;
            }
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetUserPageData service: {ex.Message}");
            return null;
        }
    }
}