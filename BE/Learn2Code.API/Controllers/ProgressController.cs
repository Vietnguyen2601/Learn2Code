using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
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
    /// Get overall course progress for current student (Student only)
    /// </summary>
    [HttpGet("courses/{courseId}/progress/me")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ServiceResult<CourseProgressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<CourseProgressDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResult<CourseProgressDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyCourseProgress(Guid courseId)
    {
        var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _progressService.GetCourseProgressAsync(courseId, studentId);
        return result.Success ? Ok(result) : StatusCode(result.Status, result);
    }

    /// <summary>
    /// Get detailed lesson progress with exercises (Student only)
    /// </summary>
    [HttpGet("lessons/{lessonId}/progress/me")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ServiceResult<LessonProgressDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<LessonProgressDetailDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResult<LessonProgressDetailDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyLessonProgress(Guid lessonId)
    {
        var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _progressService.GetLessonProgressAsync(lessonId, studentId);
        return result.Success ? Ok(result) : StatusCode(result.Status, result);
    }

    /// <summary>
    /// Update lesson progress status (Student only)
    /// </summary>
    [HttpPatch("lessons/{lessonId}/progress/me")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ServiceResult<LessonProgressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<LessonProgressDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<LessonProgressDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateMyLessonProgress(Guid lessonId, [FromBody] UpdateLessonProgressRequest request)
    {
        var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _progressService.UpdateLessonProgressAsync(lessonId, studentId, request);
        return result.Success ? Ok(result) : StatusCode(result.Status, result);
    }
}
