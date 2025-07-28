using AuthService.Application.Features.GetCurrentUser;
using Common.Contracts.Protos;
using MediatR;
using Grpc.Core;

namespace AuthService.Grpc;

public class UserGrpcService : UserService.UserServiceBase
{
    private readonly IMediator _mediator;
    
    public UserGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public override async Task<GetUserByIdResponse> GetUserById(GetUserByIdRequest request, ServerCallContext context)
    {
        var query = new GetCurrentUserQuery(request.Id);
        var result = await _mediator.Send(query, context.CancellationToken);
        
        return new GetUserByIdResponse
        {
            User = new User
            {
                Id = result.UserId,
                Email = result.Email,
                DisplayName = result.Name,
                Role = "User" // Default role for now
            }
        };
    }
} 