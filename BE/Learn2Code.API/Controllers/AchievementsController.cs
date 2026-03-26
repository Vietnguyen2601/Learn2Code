using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.AchievementDTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learn2Code.API.Controllers;

/// <summary>
/// Quản lý cả hai nhóm route:
///   - /api/achievements  (public / user)
///   - /api/admin/achievements  (Admin only)
/// </summary>
[ApiController]
public class AchievementsController : ControllerBase
{
    private readonly IAchievementService _achievementService;

    public AchievementsController(IAchievementService achievementService)
    {
        _achievementService = achievementService;
    }

    // ─── Public ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Lấy tất cả achievement (không ẩn) — catalogue công khai
    /// </summary>
    [HttpGet("api/achievements")]
    [ProducesResponseType(typeof(ServiceResult<List<AchievementDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _achievementService.GetAllAchievementsAsync();
        return Ok(result);
    }

    // ─── User (cần đăng nhập) ────────────────────────────────────────────────

    /// <summary>
    /// Lấy danh sách achievement đã unlock của user hiện tại
    /// </summary>
    [HttpGet("api/achievements/my")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResult<List<UserAchievementDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyAchievements()
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized("User ID not found in token");

        var result = await _achievementService.GetUserAchievementsAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Kiểm tra và unlock achievement sau một hành động.
    /// Gọi sau khi: hoàn thành bài học, khoá học, cập nhật streak, award XP.
    /// </summary>
    [HttpPost("api/achievements/check")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResult<UnlockResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckAndUnlock([FromBody] CheckAchievementsRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty)
            return Unauthorized("User ID not found in token");

        var result = await _achievementService.CheckAndUnlockAsync(userId, request.ConditionType);
        return Ok(result);
    }

    // ─── Admin ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Admin: Lấy tất cả achievement kể cả ẩn
    /// </summary>
    [HttpGet("api/admin/achievements")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<List<AchievementDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AdminGetAll()
    {
        var result = await _achievementService.GetAllAchievementsAdminAsync();
        return Ok(result);
    }

    /// <summary>
    /// Admin: Tạo achievement mới
    /// </summary>
    [HttpPost("api/admin/achievements")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<AchievementDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateAchievementRequest request)
    {
        var result = await _achievementService.CreateAchievementAsync(request);
        if (!result.Success)
            return result.Status == 409 ? Conflict(result) : BadRequest(result);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Admin: Cập nhật achievement
    /// </summary>
    [HttpPut("api/admin/achievements/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<AchievementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAchievementRequest request)
    {
        var result = await _achievementService.UpdateAchievementAsync(id, request);
        if (!result.Success)
        {
            return result.Status switch
            {
                404 => NotFound(result),
                409 => Conflict(result),
                _ => BadRequest(result)
            };
        }

        return Ok(result);
    }

    /// <summary>
    /// Admin: Xoá achievement
    /// </summary>
    [HttpDelete("api/admin/achievements/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _achievementService.DeleteAchievementAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Helper
    // ─────────────────────────────────────────────────────────────────────────

    private Guid GetCurrentUserId() =>
        Guid.TryParse(User.FindFirst("sub")?.Value, out var id) ? id : Guid.Empty;
}
