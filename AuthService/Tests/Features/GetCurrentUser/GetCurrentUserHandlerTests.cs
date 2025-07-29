using AuthService.Application.Features.GetCurrentUser;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AuthService.Tests.Features.GetCurrentUser;

public class GetCurrentUserHandlerTests
{
    private readonly AuthDbContext _context;
    private readonly GetCurrentUserHandler _handler;
    
    public GetCurrentUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AuthDbContext(options);
        _handler = new GetCurrentUserHandler(_context);
    }
    
    [Fact]
    public async Task Handle_WithValidUserId_ShouldReturnUserInfo()
    {
        // Arrange
        var user = new User
        {
            Id = "user123",
            GoogleId = "google123",
            Email = "test@example.com",
            Name = "Test User",
            Picture = "picture.jpg"
        };
        
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        var query = new GetCurrentUserQuery("user123");
        
        // Act
        var result = await _handler.Handle(query, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be("user123");
        result.Email.Should().Be("test@example.com");
        result.Name.Should().Be("Test User");
        result.Picture.Should().Be("picture.jpg");
    }
    
    [Fact]
    public async Task Handle_WithInvalidUserId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var query = new GetCurrentUserQuery("invalid-user-id");
        
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _handler.Handle(query, CancellationToken.None));
    }
    
    [Fact]
    public async Task Handle_WithEmptyUserId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var query = new GetCurrentUserQuery("");
        
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _handler.Handle(query, CancellationToken.None));
    }
} 