namespace AuthService.Application.Features.RefreshToken;

public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken
); 