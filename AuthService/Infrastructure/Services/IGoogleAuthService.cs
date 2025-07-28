namespace AuthService.Infrastructure.Services;

public interface IGoogleAuthService
{
    Task<GoogleUserInfo?> VerifyTokenAsync(string idToken);
}

public record GoogleUserInfo(
    string Sub,
    string Email,
    string Name,
    string? Picture,
    bool EmailVerified
); 