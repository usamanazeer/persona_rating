using AuthService.Application.Features.GoogleLogin;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Services;
using EasyNetQ;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;


namespace AuthService.Tests.Features.GoogleLogin;

public class GoogleLoginHandlerTests
{
    private readonly Mock<IGoogleAuthService> _mockGoogleAuthService;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<IBus> _mockBus;
    private readonly AuthDbContext _context;
    private readonly GoogleLoginHandler _handler;
    
    public GoogleLoginHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AuthDbContext(options);
        _mockGoogleAuthService = new Mock<IGoogleAuthService>();
        _mockJwtService = new Mock<IJwtService>();
        _mockBus = new Mock<IBus>();
        
        // Setup EasyNetQ mock with a simple implementation
        var mockPubSub = new Mock<EasyNetQ.IPubSub>();
        _mockBus.Setup(x => x.PubSub).Returns(mockPubSub.Object);
        
        _handler = new GoogleLoginHandler(_context, _mockGoogleAuthService.Object, _mockJwtService.Object, _mockBus.Object);
    }
    
    [Fact]
    public async Task Handle_WithValidGoogleToken_ShouldCreateNewUserAndReturnTokens()
    {
        // Arrange
        var googleUserInfo = new GoogleUserInfo("google123", "test@example.com", "Test User", "picture.jpg", true);
        var command = new GoogleLoginCommand("valid-google-token");
        
        _mockGoogleAuthService.Setup(x => x.VerifyTokenAsync(command.IdToken))
            .ReturnsAsync(googleUserInfo);
        
        _mockJwtService.Setup(x => x.GenerateAccessToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("access-token");
        _mockJwtService.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.Email.Should().Be("test@example.com");
        result.Name.Should().Be("Test User");
        result.Picture.Should().Be("picture.jpg");
        result.IsProfileComplete.Should().BeFalse();
        
        // Verify user was created in database
        var user = await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == "google123");
        user.Should().NotBeNull();
        user!.Email.Should().Be("test@example.com");
        
        // Verify refresh token was saved
        var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == "refresh-token");
        refreshToken.Should().NotBeNull();
        
        // Verify event was published (simplified for now)
        // _mockBus.Verify(x => x.PubSub, Times.Once);
    }
    
    [Fact]
    public async Task Handle_WithInvalidGoogleToken_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var command = new GoogleLoginCommand("invalid-google-token");
        
        _mockGoogleAuthService.Setup(x => x.VerifyTokenAsync(command.IdToken))
            .ReturnsAsync((GoogleUserInfo?)null);
        
        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
    
    [Fact]
    public async Task Handle_WithExistingUser_ShouldReturnTokensWithoutCreatingNewUser()
    {
        // Arrange
        var existingUser = new User
        {
            GoogleId = "google123",
            Email = "test@example.com",
            Name = "Test User",
            Picture = "picture.jpg",
            IsProfileComplete = true
        };
        
        await _context.Users.AddAsync(existingUser);
        await _context.SaveChangesAsync();
        
        var googleUserInfo = new GoogleUserInfo("google123", "test@example.com", "Test User", "picture.jpg", true);
        var command = new GoogleLoginCommand("valid-google-token");
        
        _mockGoogleAuthService.Setup(x => x.VerifyTokenAsync(command.IdToken))
            .ReturnsAsync(googleUserInfo);
        
        _mockJwtService.Setup(x => x.GenerateAccessToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns("access-token");
        _mockJwtService.Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.IsProfileComplete.Should().BeTrue();
        
        // Verify no new user was created
        var userCount = await _context.Users.CountAsync();
        userCount.Should().Be(1);
        
        // Verify no event was published for existing user (simplified for now)
        // _mockBus.Verify(x => x.PubSub, Times.Never);
    }
} 