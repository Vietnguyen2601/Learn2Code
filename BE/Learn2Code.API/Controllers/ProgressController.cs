using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.ProgressDTOs.ProgressRequests;
using Learn2Code.Application.DTOs.ProgressDTOs.ProgressResponses;
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
    /// Get overall course progress for current student or specific student (Admin can pass studentId query param)
    /// </summary>
    [HttpGet("courses/{courseId}/progress/me")]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(ServiceResult<CourseProgressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<CourseProgressDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResult<CourseProgressDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyCourseProgress(Guid courseId, [FromQuery] Guid? studentId)
    {
        var requestedStudentId = studentId ?? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _progressService.GetCourseProgressAsync(courseId, requestedStudentId);
        return result.Success ? Ok(result) : StatusCode(result.Status, result);
    }

    /// <summary>
    /// Get course progress for a specific student (Admin only)
    /// </summary>
    [HttpGet("courses/{courseId}/progress/{studentId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<CourseProgressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<CourseProgressDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResult<CourseProgressDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCourseProgressForStudent(Guid courseId, Guid studentId)
    {
        var result = await _progressService.GetCourseProgressAsync(courseId, studentId);
        return result.Success ? Ok(result) : StatusCode(result.Status, result);
    }

    /// <summary>
    /// Get detailed lesson progress with exercises (Admin can pass studentId query param)
    /// </summary>
    [HttpGet("lessons/{lessonId}/progress/me")]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(ServiceResult<LessonProgressDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<LessonProgressDetailDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResult<LessonProgressDetailDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyLessonProgress(Guid lessonId, [FromQuery] Guid? studentId)
    {
        var requestedStudentId = studentId ?? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _progressService.GetLessonProgressAsync(lessonId, requestedStudentId);
        return result.Success ? Ok(result) : StatusCode(result.Status, result);
    }

    /// <summary>
    /// Get detailed lesson progress for a specific student (Admin only)
    /// </summary>
    [HttpGet("lessons/{lessonId}/progress/{studentId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<LessonProgressDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<LessonProgressDetailDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResult<LessonProgressDetailDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLessonProgressForStudent(Guid lessonId, Guid studentId)
    {
        var result = await _progressService.GetLessonProgressAsync(lessonId, studentId);
        return result.Success ? Ok(result) : StatusCode(result.Status, result);
    }

    /// <summary>
    /// Update lesson progress status (Admin can pass studentId query param)
    /// </summary>
    [HttpPatch("lessons/{lessonId}/progress/me")]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(ServiceResult<LessonProgressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<LessonProgressDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<LessonProgressDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateMyLessonProgress(Guid lessonId, [FromBody] UpdateLessonProgressRequest request, [FromQuery] Guid? studentId)
    {
        var requestedStudentId = studentId ?? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _progressService.UpdateLessonProgressAsync(lessonId, requestedStudentId, request);
        return result.Success ? Ok(result) : StatusCode(result.Status, result);
    }
}
