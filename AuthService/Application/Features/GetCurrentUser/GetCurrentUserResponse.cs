namespace AuthService.Application.Features.GetCurrentUser;

public record GetCurrentUserResponse(
    string UserId,
    string Email,
    string Name,
    string? Picture,
    bool IsProfileComplete
); 