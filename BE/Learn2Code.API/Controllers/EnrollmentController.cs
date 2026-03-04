using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/enrollments")]
[Authorize]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    /// <summary>
    /// Get all enrollments for current student
    /// </summary>
    [HttpGet("me")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyEnrollments()
    {
        var studentId = GetCurrentUserId();
        var result = await _enrollmentService.GetMyEnrollmentsAsync(studentId);
        return Ok(result);
    }

    /// <summary>
    /// Enroll in a course
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> EnrollCourse([FromBody] CreateEnrollmentRequest request)
    {
        var studentId = GetCurrentUserId();
        var result = await _enrollmentService.EnrollCourseAsync(studentId, request);
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }

    /// <summary>
    /// Get enrollment detail with progress for a specific course
    /// </summary>
    [HttpGet("me/{courseId}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyEnrollmentDetail(Guid courseId)
    {
        var studentId = GetCurrentUserId();
        var result = await _enrollmentService.GetMyEnrollmentDetailAsync(studentId, courseId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Get all enrollments (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllEnrollments()
    {
        var result = await _enrollmentService.GetAllEnrollmentsAsync();
        return Ok(result);
    }

    #region Helper Methods

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found"));
    }

    #endregion
}
