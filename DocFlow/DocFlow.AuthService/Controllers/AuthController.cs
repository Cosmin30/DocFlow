using DocFlow.AuthService.Application.Contracts;
using DocFlow.AuthService.Application.Services;
using DocFlow.BuildingBlocks.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocFlow.AuthService.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await authService.RegisterAsync(request, cancellationToken);
            return result.Success
                ? Created(string.Empty, new { message = "User registered." })
                : BadRequest(new { message = result.Message });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 500);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var device = HttpContext.Request.Headers.UserAgent.ToString();
            var result = await authService.LoginAsync(request, ip, device, cancellationToken);

            return result.Success ? Ok(result.Payload) : Unauthorized(new { message = result.Message });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 500);
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await authService.RefreshAsync(request, cancellationToken);
            return result.Success ? Ok(result.Payload) : Unauthorized(new { message = result.Message });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 500);
        }
    }

    [Authorize]
    [HttpPost("logout-all")]
    public async Task<IActionResult> LogoutAll(CancellationToken cancellationToken)
    {
        try
        {
            await authService.LogoutAllAsync(User.GetUserId(), cancellationToken);
            return Ok(new { message = "All devices logged out." });
        }
        catch (Exception ex)
        {
            return Problem(ex.Message, statusCode: 500);
        }
    }
}
