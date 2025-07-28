using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using AuthService.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Application.Features.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly AuthDbContext _context;
    private readonly IJwtService _jwtService;
    
    public RefreshTokenHandler(AuthDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }
    
    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Find the refresh token
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);
        
        if (refreshToken == null || refreshToken.RevokedAt.HasValue || refreshToken.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        
        // Generate new tokens
        var newAccessToken = _jwtService.GenerateAccessToken(refreshToken.UserId, refreshToken.User.Email, "User");
        var newRefreshTokenValue = _jwtService.GenerateRefreshToken();
        
        // Revoke old refresh token
        refreshToken.RevokedAt = DateTime.UtcNow;
        
        // Create new refresh token
        var newRefreshToken = new AuthService.Domain.Entities.RefreshToken
        {
            UserId = refreshToken.UserId,
            Token = newRefreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        
        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return new RefreshTokenResponse(newAccessToken, newRefreshTokenValue);
    }
} 