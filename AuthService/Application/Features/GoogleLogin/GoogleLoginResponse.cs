namespace AuthService.Application.Features.GoogleLogin;

public record GoogleLoginResponse(
    string AccessToken,
    string RefreshToken,
    string UserId,
    string Email,
    string Name,
    string? Picture
); 