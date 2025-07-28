using AuthService.Application.Features.RefreshToken;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace AuthService.Tests.Features.RefreshToken;

public class RefreshTokenHandlerTests
{
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly AuthDbContext _context;
    private readonly RefreshTokenHandler _handler;
    
    public RefreshTokenHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AuthDbContext(options);
        _mockJwtService = new Mock<IJwtService>();
        
        _handler = new RefreshTokenHandler(_context, _mockJwtService.Object);
    }
    
    [Fact]
    public async Task Handle_WithValidRefreshToken_ShouldReturnNewTokens()
    {
        // Arrange
        var user = new User
        {
            Id = "user123",
            GoogleId = "google123",
            Email = "test@example.com",
            Name = "Test User"
        };
        
        var refreshToken = new AuthService.Domain.Entities.RefreshToken
        {
            Id = "rt123",
            UserId = "user123",
            Token = "valid-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        
        await _context.Users.AddAsync(user);
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
        
        var command = new RefreshTokenCommand("valid-refresh-token");
        
        _mockJwtService.Setup(x => x.GenerateAccessToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("new-access-token");
        _mockJwtService.Setup(x => x.GenerateRefreshToken())
            .Returns("new-refresh-token");
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("new-access-token");
        result.RefreshToken.Should().Be("new-refresh-token");
        
        // Verify old refresh token was revoked
        var oldRefreshToken = await _context.RefreshTokens.FindAsync("rt123");
        oldRefreshToken!.RevokedAt.Should().NotBeNull();
        
        // Verify new refresh token was created
        var newRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == "new-refresh-token");
        newRefreshToken.Should().NotBeNull();
        newRefreshToken!.RevokedAt.Should().BeNull();
    }
    
    [Fact]
    public async Task Handle_WithInvalidRefreshToken_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var command = new RefreshTokenCommand("invalid-refresh-token");
        
        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
    
    [Fact]
    public async Task Handle_WithExpiredRefreshToken_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var user = new User
        {
            Id = "user123",
            GoogleId = "google123",
            Email = "test@example.com",
            Name = "Test User"
        };
        
        var refreshToken = new AuthService.Domain.Entities.RefreshToken
        {
            Id = "rt123",
            UserId = "user123",
            Token = "expired-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(-1) // Expired
        };
        
        await _context.Users.AddAsync(user);
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
        
        var command = new RefreshTokenCommand("expired-refresh-token");
        
        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
    
    [Fact]
    public async Task Handle_WithRevokedRefreshToken_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var user = new User
        {
            Id = "user123",
            GoogleId = "google123",
            Email = "test@example.com",
            Name = "Test User"
        };
        
        var refreshToken = new AuthService.Domain.Entities.RefreshToken
        {
            Id = "rt123",
            UserId = "user123",
            Token = "revoked-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            RevokedAt = DateTime.UtcNow // Already revoked
        };
        
        await _context.Users.AddAsync(user);
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
        
        var command = new RefreshTokenCommand("revoked-refresh-token");
        
        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
} 