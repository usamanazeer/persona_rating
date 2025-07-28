using MediatR;

namespace AuthService.Application.Features.GoogleLogin;

public record GoogleLoginCommand(
    string IdToken
) : IRequest<GoogleLoginResponse>; 