using System.Security.Claims;

namespace AuthService.Infrastructure.Services;

public interface IJwtService
{
    string GenerateAccessToken(string userId, string email, string role);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
    string GetUserIdFromToken(string token);
} 