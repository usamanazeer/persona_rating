using AuthService.Application.Features.GoogleLogin;
using AuthService.Application.Features.RefreshToken;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("google-login")]
    public async Task<ActionResult<GoogleLoginResponse>> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        try
        {
            var command = new GoogleLoginCommand(request.IdToken);
            GoogleLoginResponse result = await _mediator.Send(command);

      
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
    
    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var command = new RefreshTokenCommand(request.RefreshToken);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}

public record GoogleLoginRequest(string IdToken);
public record RefreshTokenRequest(string RefreshToken); 