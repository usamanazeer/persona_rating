using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace AuthService.Infrastructure.Services;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    
    public GoogleAuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }
    
    public async Task<GoogleUserInfo?> VerifyTokenAsync(string idToken)
    {
        try
        {
            // In production, you should verify the token signature
            // For MVP, we'll make a simple HTTP call to Google's userinfo endpoint
            var response = await _httpClient.GetAsync($"https://www.googleapis.com/oauth2/v3/userinfo?access_token={idToken}");
            
            if (!response.IsSuccessStatusCode)
                return null;
                
            var json = await response.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<GoogleUserInfo>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            return userInfo;
        }
        catch
        {
            return null;
        }
    }
} 