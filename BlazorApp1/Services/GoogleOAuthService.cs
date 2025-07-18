using System.Text.Json;
using BlazorApp1.Models.Auth;
using Microsoft.JSInterop;

namespace BlazorApp1.Services;

public class GoogleOAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IJSRuntime _jsRuntime;

    public GoogleOAuthService(HttpClient httpClient, IConfiguration configuration, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _jsRuntime = jsRuntime;
    }

    public async Task<TokenResponse> ExchangeCodeForTokenAsync(string code)
    {
        var clientId = _configuration["GoogleClientId"];
        var redirectUri = "http://localhost:7032/login";
        
        var tokenRequest = new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["code"] = code,
            ["grant_type"] = "authorization_code",
            ["redirect_uri"] = redirectUri
        };

        var content = new FormUrlEncodedContent(tokenRequest);
        
        var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Token exchange failed: {responseContent}");

        return JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower 
        });
    }
    
    public async Task<GoogleAuthResult> SignInAsync()
    {
        var clientId = _configuration["Google:ClientId"];
        var redirectUri = "http://localhost:7032/login";
        
        var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                      $"client_id={clientId}&" +
                      $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                      $"response_type=code&" +
                      $"scope=openid%20profile%20email&" +
                      $"access_type=offline";

        await _jsRuntime.InvokeVoidAsync("open", authUrl, "_self");
        return new GoogleAuthResult { Success = false }; // Will redirect, so this won't be reached
    }

}