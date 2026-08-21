using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.DTOs.Auth;
using TaskManagement.Api.Services;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("Signup")]
    [ProducesResponseType(typeof(SignupResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SignupResponseDto>> Signup([FromBody] SignupRequestDto request)
    {
        var result = await _authService.SignupAsync(request);

        if (!result.Success)
        {
            return BadRequest(new { message = result.Error });
        }

        return StatusCode(StatusCodes.Status201Created, result.Data);
    }

    [HttpPost("Login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (!result.Success)
        {
            return Unauthorized(new { message = result.Error });
        }

        return Ok(result.Data);
    }
}
