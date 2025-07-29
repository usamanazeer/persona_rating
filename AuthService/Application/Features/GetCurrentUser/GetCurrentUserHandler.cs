using AuthService.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Application.Features.GetCurrentUser;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, GetCurrentUserResponse>
{
    private readonly AuthDbContext _context;
    
    public GetCurrentUserHandler(AuthDbContext context)
    {
        _context = context;
    }
    
    public async Task<GetCurrentUserResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        
        if (user == null)
            throw new KeyNotFoundException("User not found");
        
        return new GetCurrentUserResponse(
            user.Id,
            user.Email,
            user.Name,
            user.Picture
        );
    }
} 