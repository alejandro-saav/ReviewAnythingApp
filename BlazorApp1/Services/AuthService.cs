using BlazorApp1.Models.Auth;
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
        _httpClient = httpClientFactory.CreateClient("BlazorAppApi");
        _navigationManager = navigationManager;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        LastErrorMessage = null;
        try
        {
            Uri requestUri = new Uri(_httpClient.BaseAddress!, "auth/Login");
            var response = await _httpClient.PostAsJsonAsync(requestUri, request);
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
            var response = await _httpClient.PostAsJsonAsync("auth/Register", request);
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
            Uri requestUri = new Uri(_httpClient.BaseAddress!, $"auth/confirm-email?userId={userId}&token={encodedToken}");
            var response = await _httpClient.GetAsync(requestUri);
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
            Uri requestUri = new Uri(_httpClient.BaseAddress!, $"auth/forgot-password");
            var response = await _httpClient.PostAsJsonAsync(requestUri, request);
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
            Uri requestUri = new Uri(_httpClient.BaseAddress!, $"auth/reset-password?userId={userId}&token={encodedToken}");
            var response = await _httpClient.PostAsJsonAsync(requestUri, newPassword);
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