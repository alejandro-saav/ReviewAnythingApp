using BlazorApp1.Models.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity.Data;
using LoginRequest = BlazorApp1.Models.Auth.LoginRequest;

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

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        LastErrorMessage = null;
        try
        {
            Console.WriteLine($"AuthService HttpClient BaseAddress: {_httpClient.BaseAddress}");

            // --- CHANGE IS HERE ---
            // Create a full absolute URI using the BaseAddress
            Uri requestUri = new Uri(_httpClient.BaseAddress!, "api/auth/Login");
            //var response = await _httpClient.PostAsJsonAsync("api/auth/Login", request);
            var response = await _httpClient.PostAsJsonAsync(requestUri, request);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {

                var errorContent = await response.Content.ReadAsStringAsync();
                LastErrorMessage = $"Login failed: {response.StatusCode}. {errorContent}";
                return false;
            }
        }
        catch (Exception ex)
        {
            LastErrorMessage = $"Network error during login: {ex.Message}";
            return false;
        }
    }

    public async Task<bool> RegisterAsync(RegisterRequestDto request)
    {
        LastErrorMessage = null;
        try
        {
            Uri requestUri = new Uri(_httpClient.BaseAddress!, "api/auth/Register");
            var response = await _httpClient.PostAsJsonAsync(requestUri, request);
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
            // var encodedToken = Uri.EscapeDataString(token);
            Uri requestUri = new Uri(_httpClient.BaseAddress!, $"api/auth/confirm-email?userId={userId}&token={token}");
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
}