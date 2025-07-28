namespace AuthService.Domain.Events;

public record UserCreatedEvent(
    string UserId,
    string Email,
    string Name,
    string? Picture,
    DateTime CreatedAt
); 