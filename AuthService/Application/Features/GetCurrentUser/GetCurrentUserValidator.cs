using FluentValidation;

namespace AuthService.Application.Features.GetCurrentUser;

public class GetCurrentUserValidator : AbstractValidator<GetCurrentUserQuery>
{
    public GetCurrentUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
} 