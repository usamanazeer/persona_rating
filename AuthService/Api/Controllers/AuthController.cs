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

        //    var result = new GoogleLoginResponse(
        //        AccessToken: "ya29.a0AfH6SMDMOCKTOKEN1234567890",
        //        RefreshToken: "1//0gMOCKREFRESHTOKEN1234567890",
        //        UserId: "123456789012345678901",
        //        Email: "john.doe@example.com",
        //        Name: "John Doe",
        //        Picture: "https://lh3.googleusercontent.com/a-/AOh14GgMockProfilePic",
        //        IsProfileComplete: true
        //);
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