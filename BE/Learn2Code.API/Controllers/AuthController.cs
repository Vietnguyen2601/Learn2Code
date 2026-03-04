using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    /// Send OTP to email for registration
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Verify OTP and create account
    /// </summary>
    [HttpPost("verify-otp")]
    [ProducesResponseType(typeof(ServiceResult<VerifyOtpResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResult<VerifyOtpResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        var result = await _authService.VerifyOtpAsync(request);
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
    }

    /// <summary>
    /// Get a new access_token from refresh_token
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ServiceResult<RefreshTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<RefreshTokenResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<RefreshTokenResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request);

        if (result.Success)
        {
            return Ok(result);
        }

        return result.ErrorCode switch
        {
            "INVALID_TOKEN" => Unauthorized(result),
            "INVALID_REFRESH_TOKEN" => Unauthorized(result),
            "ACCOUNT_INACTIVE" => Unauthorized(result),
            _ => BadRequest(result)
        };
    }

    /// <summary>
    /// Get current account info (requires authentication)
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(ServiceResult<MeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMe()
    {
        var accountId = GetCurrentAccountId();
        if (accountId == null) return Unauthorized();

        var result = await _authService.GetMeAsync(accountId.Value);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Update profile — name and/or phone_number (requires authentication)
    /// </summary>
    [Authorize]
    [HttpPatch("me")]
    [ProducesResponseType(typeof(ServiceResult<UpdateProfileResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<UpdateProfileResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var accountId = GetCurrentAccountId();
        if (accountId == null) return Unauthorized();

        var result = await _authService.UpdateProfileAsync(accountId.Value, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Logout — revokes the refresh token (requires authentication)
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        var accountId = GetCurrentAccountId();
        if (accountId == null) return Unauthorized();

        var result = await _authService.LogoutAsync(accountId.Value);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // ─── helpers ───────────────────────────────────────────────────────────────
    private Guid? GetCurrentAccountId()
    {
        var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? User.FindFirst("sub")?.Value;
        return Guid.TryParse(value, out var id) ? id : null;
    }
}