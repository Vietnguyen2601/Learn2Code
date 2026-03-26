using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.GamificationDTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learn2Code.API.Controllers;

/// <summary>
/// Quản lý Daily Streak — khuyến khích học viên học liên tục hàng ngày.
/// </summary>
[ApiController]
public class DailyStreakController : ControllerBase
{
    private readonly IDailyStreakService _streakService;

    public DailyStreakController(IDailyStreakService streakService)
    {
        _streakService = streakService;
    }

    // ─── User ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Lấy thông tin streak hiện tại của user: current, longest, history 30 ngày
    /// </summary>
    [HttpGet("api/streaks/me")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResult<DailyStreakDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyStreak()
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized("User ID not found in token");

        var result = await _streakService.GetUserStreakAsync(userId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Check-in daily streak.
    /// Gọi tự động sau khi user hoàn thành Exercise (GradedCode) hoặc SectionQuiz.
    /// NOT for public use — chỉ gọi từ backend (fire-and-forget).
    /// </summary>
    [HttpPost("api/streaks/check-in")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResult<CheckInStreakResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckIn()
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized("User ID not found in token");

        var result = await _streakService.CheckInAsync(userId);
        return result.Success ? Ok(result) : (result.Status == 404 ? NotFound(result) : Ok(result));
    }

    // ─── Admin ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Admin: Lấy thông tin streak của một user
    /// </summary>
    [HttpGet("api/admin/streaks/{userId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<DailyStreakDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserStreakAdmin(Guid userId)
    {
        var result = await _streakService.GetUserStreakAsync(userId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Helper
    // ─────────────────────────────────────────────────────────────────────────

    private Guid GetCurrentUserId() =>
        Guid.TryParse(User.FindFirst("sub")?.Value, out var id) ? id : Guid.Empty;
}
