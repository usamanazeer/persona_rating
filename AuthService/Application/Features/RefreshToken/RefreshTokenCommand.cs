using MediatR;

namespace AuthService.Application.Features.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<RefreshTokenResponse>; 