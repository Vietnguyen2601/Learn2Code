using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api")]
public class ProgressController : ControllerBase
{
    private readonly IProgressService _progressService;

    public ProgressController(IProgressService progressService)
    {
        _progressService = progressService;
    }

    /// <summary>
    /// Get my course progress overview (all sections, lessons, quizzes)
    /// </summary>
    [HttpGet("courses/{courseId}/progress/me")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyCourseProgress(Guid courseId)
    {
        var studentId = GetCurrentUserId();
        var result = await _progressService.GetMyCourseProgressAsync(studentId, courseId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get my lesson progress detail (exercises and quiz status)
    /// </summary>
    [HttpGet("lessons/{lessonId}/progress/me")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyLessonProgress(Guid lessonId)
    {
        var studentId = GetCurrentUserId();
        var result = await _progressService.GetMyLessonProgressAsync(studentId, lessonId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get all students' progress in a course (Admin only)
    /// </summary>
    [HttpGet("courses/{courseId}/progress")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllStudentsProgress(Guid courseId)
    {
        var result = await _progressService.GetAllStudentsProgressAsync(courseId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    #region Helper Methods

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found"));
    }

    #endregion
}
