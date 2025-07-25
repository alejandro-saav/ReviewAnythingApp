using System.Text.Json;
using BlazorApp1.Models.Auth;
using Microsoft.JSInterop;

namespace BlazorApp1.Services;

public class GoogleOAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GoogleOAuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<TokenResponse> ExchangeCodeForTokenAsync(string code)
    {
        var clientId = _configuration["Google:ClientId"];
        var clientSecret = _configuration["Google:ClientSecret"];
        var redirectUri = _configuration["Google:RedirectUrl"];
        var tokenRequest = new Dictionary<string, string>
        {
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
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

}