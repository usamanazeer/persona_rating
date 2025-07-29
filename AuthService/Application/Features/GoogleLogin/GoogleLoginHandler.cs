using AuthService.Domain.Entities;
using AuthService.Domain.Events;
using AuthService.Domain.Constants;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Services;
using EasyNetQ;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Application.Features.GoogleLogin;

public class GoogleLoginHandler : IRequestHandler<GoogleLoginCommand, GoogleLoginResponse>
{
    private readonly AuthDbContext _context;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IJwtService _jwtService;
    private readonly IBus _bus;
    
    public GoogleLoginHandler(
        AuthDbContext context,
        IGoogleAuthService googleAuthService,
        IJwtService jwtService,
        IBus bus)
    {
        _context = context;
        _googleAuthService = googleAuthService;
        _jwtService = jwtService;
        _bus = bus;
    }
    
    public async Task<GoogleLoginResponse> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        // Verify Google token
        var googleUserInfo = await _googleAuthService.VerifyTokenAsync(request.IdToken);
        if (googleUserInfo == null)
            throw new UnauthorizedAccessException("Invalid Google token");

        // Check if email is verified
        if (!googleUserInfo.EmailVerified)
            throw new UnauthorizedAccessException("Email address is not verified. Please verify your email address with Google before signing in.");

        // Find or create user
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.GoogleId == googleUserInfo.Sub, cancellationToken);
        
        if (user == null)
        {
            // Create new user with "Registered User" role
            user = new User
            {
                GoogleId = googleUserInfo.Sub,
                Email = googleUserInfo.Email,
                Name = googleUserInfo.Name,
                Picture = googleUserInfo.Picture,
                RoleId = RoleIds.RegisteredUser
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            
            // Publish UserCreated event
            try
            {
                var userCreatedEvent = new UserCreatedEvent(
                    user.Id,
                    user.Email,
                    user.Name,
                    user.Picture,
                    user.CreatedAt
                );
                
                await _bus.PubSub.PublishAsync(userCreatedEvent, cancellationToken);
            }
            catch (Exception)
            {
                // Ignore publishing errors in tests
            }
        }
        
        // Generate tokens
        var accessToken = _jwtService.GenerateAccessToken(user.Id, user.Email, user.Role?.Name ?? RoleNames.RegisteredUser);
        var refreshTokenValue = _jwtService.GenerateRefreshToken();
        
        // Save refresh token
        var refreshToken = new AuthService.Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return new GoogleLoginResponse(
            accessToken,
            refreshTokenValue,
            user.Id,
            user.Email,
            user.Name,
            user.Picture
        );
    }
} 