using FluentValidation;

namespace AuthService.Application.Features.GoogleLogin;

public class GoogleLoginValidator : AbstractValidator<GoogleLoginCommand>
{
    public GoogleLoginValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty()
            .WithMessage("Google ID token is required");
    }
} 