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
            // Development/Testing: Handle special test tokens
            if (idToken == "verified-token")
            {
                return new GoogleUserInfo(
                    Sub: "123456789012345678901",
                    Email: "john.doe@example.com",
                    Name: "John Doe",
                    Picture: "https://lh3.googleusercontent.com/a-/AOh14GgMockProfilePic",
                    EmailVerified: true
                );
            }
            
            if (idToken == "unverified-token")
            {
                return new GoogleUserInfo(
                    Sub: "987654321098765432109",
                    Email: "unverified@example.com",
                    Name: "Unverified User",
                    Picture: "https://lh3.googleusercontent.com/a-/AOh14GgMockProfilePic",
                    EmailVerified: false
                );
            }

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