using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register new account with OTP verification
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ServiceResult<RegisterResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResult<RegisterResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }

    /// <summary>
    /// Login with email/username and password
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ServiceResult<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<LoginResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<LoginResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (result.Success)
        {
            return Ok(result);
        }

        return result.ErrorCode switch
        {
            "INVALID_CREDENTIALS" => Unauthorized(result),
            "ACCOUNT_INACTIVE" => Unauthorized(result),
            _ => BadRequest(result)
        };
    }

    /// <summary>
    /// Send OTP for password reset
    /// </summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var result = await _authService.ForgotPasswordAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Reset password with OTP verification
    /// </summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ServiceResult<ResetPasswordResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<ResetPasswordResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }}