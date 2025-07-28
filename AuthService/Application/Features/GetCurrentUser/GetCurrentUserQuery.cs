using MediatR;

namespace AuthService.Application.Features.GetCurrentUser;

public record GetCurrentUserQuery(
    string UserId
) : IRequest<GetCurrentUserResponse>; 