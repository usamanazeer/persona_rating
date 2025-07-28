using AuthService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using FluentAssertions;
using Xunit;

namespace AuthService.Tests.Services;

public class JwtServiceTests
{
    private readonly JwtService _jwtService;
    
    public JwtServiceTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Jwt:SecretKey"] = "test-secret-key-for-unit-testing-only",
                ["Jwt:Issuer"] = "test-issuer",
                ["Jwt:Audience"] = "test-audience"
            })
            .Build();
        
        _jwtService = new JwtService(configuration);
    }
    
    [Fact]
    public void GenerateAccessToken_ShouldReturnValidToken()
    {
        // Arrange
        var userId = "user123";
        var email = "test@example.com";
        var role = "User";
        
        // Act
        var token = _jwtService.GenerateAccessToken(userId, email, role);
        
        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Should().Contain(".");
    }
    
    [Fact]
    public void GenerateRefreshToken_ShouldReturnValidToken()
    {
        // Act
        var token = _jwtService.GenerateRefreshToken();
        
        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Length.Should().BeGreaterThan(50); // Base64 encoded token should be long
    }
    
    [Fact]
    public void ValidateToken_WithValidToken_ShouldReturnClaimsPrincipal()
    {
        // Arrange
        var userId = "user123";
        var email = "test@example.com";
        var role = "User";
        var token = _jwtService.GenerateAccessToken(userId, email, role);
        
        // Act
        var principal = _jwtService.ValidateToken(token);
        
        // Assert
        principal.Should().NotBeNull();
        principal!.FindFirst("userId")?.Value.Should().Be(userId);
        principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value.Should().Be(email);
        principal.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value.Should().Be(role);
    }
    
    [Fact]
    public void ValidateToken_WithInvalidToken_ShouldReturnNull()
    {
        // Arrange
        var invalidToken = "invalid.token.here";
        
        // Act
        var principal = _jwtService.ValidateToken(invalidToken);
        
        // Assert
        principal.Should().BeNull();
    }
    
    [Fact]
    public void GetUserIdFromToken_WithValidToken_ShouldReturnUserId()
    {
        // Arrange
        var userId = "user123";
        var email = "test@example.com";
        var role = "User";
        var token = _jwtService.GenerateAccessToken(userId, email, role);
        
        // Act
        var result = _jwtService.GetUserIdFromToken(token);
        
        // Assert
        result.Should().Be(userId);
    }
    
    [Fact]
    public void GetUserIdFromToken_WithInvalidToken_ShouldReturnEmptyString()
    {
        // Arrange
        var invalidToken = "invalid.token.here";
        
        // Act
        var result = _jwtService.GetUserIdFromToken(invalidToken);
        
        // Assert
        result.Should().BeEmpty();
    }
} 