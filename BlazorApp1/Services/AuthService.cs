using BlazorApp1.Models.Auth;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigationManager;
    //private readonly ILocalStorageService _localStorage;
    
    public string? LastErrorMessage { get; private set; }

    public AuthService(IHttpClientFactory httpClientFactory, NavigationManager navigationManager)
    {
        _httpClient = httpClientFactory.CreateClient("ReviewAnythingAPI");
        _navigationManager = navigationManager;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/Login",request);
            if (response.IsSuccessStatusCode)
            {
                LoginResponse loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return loginResponse;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                LastErrorMessage = $"Login failed: {response.StatusCode}. {errorContent}";
                return new LoginResponse { ErrorMessage = errorContent, Success = false };
            }
        }
        catch (Exception ex)
        {
            LastErrorMessage = $"Network error during login: {ex.Message}";
            return new LoginResponse { ErrorMessage = ex.Message, Success = false };
        }
    }

    public async Task<bool> RegisterAsync(RegisterRequestDto request)
    {
        LastErrorMessage = null;
        try
        {
            using var content = new MultipartFormDataContent();
            content.Add(new  StringContent(request.UserName), "UserName");
            content.Add(new  StringContent(request.Email), "Email");
            content.Add(new  StringContent(request.FirstName), "FirstName");
            content.Add(new  StringContent(request.LastName ?? ""), "LastName");
            content.Add(new  StringContent(request.Password), "Password");
            content.Add(new  StringContent(request.Phone ?? ""), "Phone");
            content.Add(new  StringContent(request.Bio ?? ""), "Bio");

            if (request.ProfileImage != null)
            {
                const long maxFileSize = 1024 * 1024 * 2;
                var fileStream = request.ProfileImage.OpenReadStream(maxFileSize);
                var streamContent = new StreamContent(fileStream);
                content.Add(streamContent, "ProfileImage", request.ProfileImage.Name);
            }
            var response = await _httpClient.PostAsync("api/auth/Register", content);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                LastErrorMessage = $"Signup failed: {response.StatusCode}. {errorContent}";
                return false;
            }
        }
        catch (Exception ex)
        {
            LastErrorMessage = $"Network error during signup: {ex.Message}";
            return false;
        }
    }

    public async Task<bool> ConfirmEmailAsync(string userId, string token)
    {
        try
        {
            var encodedToken = Uri.EscapeDataString(token);
            var response = await _httpClient.GetAsync($"api/auth/confirm-email?userId={userId}&token={encodedToken}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                LastErrorMessage = $"Confirm email failed: {response.StatusCode}.";
                return false;
            }
        }
        catch (Exception ex)
        {
            LastErrorMessage = $"Network error during signup: {ex.Message}";
            return false;
        }
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/forgot-password", request);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                LastErrorMessage = $"Error: {response.StatusCode}. {errorContent}";
                return false;
            }
        }
        catch (Exception ex)
        {
            LastErrorMessage = $"Network error during forgot-password: {ex.Message}";
            return false;
        }
    }

    public async Task<bool> ResetPasswordAsync(string userId, string token, string newPassword)
    {
        LastErrorMessage = null;
        try
        {
            var encodedToken = Uri.EscapeDataString(token);
            var response = await _httpClient.PostAsJsonAsync($"api/auth/reset-password?userId={userId}&token={encodedToken}", newPassword);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                LastErrorMessage = $"Error: {response.StatusCode}. {errorContent}";
                return false;
            }
        }
        catch (Exception ex)
        {
            LastErrorMessage = $"Network error during forgot-password: {ex.Message}";
            return false;
        }
    }
}