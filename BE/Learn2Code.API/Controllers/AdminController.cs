using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.AdminDTOs.AdminRequests;
using Learn2Code.Application.DTOs.AdminDTOs.AdminResponses;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    // ─── User Management ───────────────────────────

    /// <summary>
    /// Get all users (filter by search, role, is_active)
    /// </summary>
    [HttpGet("users")]
    [ProducesResponseType(typeof(ServiceResult<List<AdminUserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] string? search,
        [FromQuery] string? role,
        [FromQuery] bool? is_active)
    {
        var result = await _adminService.GetAllUsersAsync(search, role, is_active);
        return Ok(result);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("users/{id}")]
    [ProducesResponseType(typeof(ServiceResult<AdminUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<AdminUserDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await _adminService.GetUserByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Update user (role, is_active)
    /// </summary>
    [HttpPatch("users/{id}")]
    [ProducesResponseType(typeof(ServiceResult<AdminUserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<AdminUserDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<AdminUserDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var result = await _adminService.UpdateUserAsync(id, request);
        if (result.Success) return Ok(result);
        return result.Status == 404 ? NotFound(result) : BadRequest(result);
    }

    // ─── Dashboard ─────────────────────────────────

    /// <summary>
    /// Get dashboard overview (total users, courses, enrollments, revenue)
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(ServiceResult<DashboardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetDashboard()
    {
        var result = await _adminService.GetDashboardAsync();
        return Ok(result);
    }

    /// <summary>
    /// Get revenue by month (last N months)
    /// </summary>
    [HttpGet("dashboard/revenue")]
    [ProducesResponseType(typeof(ServiceResult<List<RevenueByMonthDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetRevenue([FromQuery] int months = 12)
    {
        var result = await _adminService.GetRevenueByMonthAsync(months);
        return Ok(result);
    }

    /// <summary>
    /// Get enrollment count by course
    /// </summary>
    [HttpGet("dashboard/enrollments")]
    [ProducesResponseType(typeof(ServiceResult<List<EnrollmentByCourseDashboardDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetEnrollmentsByCourse()
    {
        var result = await _adminService.GetEnrollmentsByCourseDashboardAsync();
        return Ok(result);
    }
}
