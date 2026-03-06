using System.Security.Claims;
using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api")]
public class LessonController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    /// <summary>
    /// Get all lessons in a section (Admin & Student)
    /// </summary>
    [HttpGet("sections/{sectionId}/lessons")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResult<List<LessonDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<List<LessonDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLessonsBySection(Guid sectionId)
    {
        var result = await _lessonService.GetLessonsBySectionIdAsync(sectionId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Get lesson detail by ID (Admin & Student - with access control)
    /// </summary>
    [HttpGet("lessons/{lessonId}")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResult<LessonDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<LessonDetailDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResult<LessonDetailDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResult<LessonDetailDto>),StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetLessonById(Guid lessonId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid? userId = userIdClaim != null ? Guid.Parse(userIdClaim) : null;

        var result = await _lessonService.GetLessonByIdAsync(lessonId, userId);
        
        if (!result.Success)
        {
            return result.Status == 403 ? StatusCode(403, result) : NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new lesson in section (Admin only)
    /// </summary>
    [HttpPost("sections/{sectionId}/lessons")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<LessonDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResult<LessonDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<LessonDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateLesson(Guid sectionId, [FromBody] CreateLessonRequest request)
    {
        var result = await _lessonService.CreateLessonAsync(sectionId, request);
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }

    /// <summary>
    /// Update a lesson (Admin only)
    /// </summary>
    [HttpPatch("lessons/{lessonId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<LessonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<LessonDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<LessonDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateLesson(Guid lessonId, [FromBody] UpdateLessonRequest request)
    {
        var result = await _lessonService.UpdateLessonAsync(lessonId, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Delete a lesson (Admin only)
    /// </summary>
    [HttpDelete("lessons/{lessonId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteLesson(Guid lessonId)
    {
        var result = await _lessonService.DeleteLessonAsync(lessonId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Reorder lessons in a section (Admin only)
    /// </summary>
    [HttpPatch("sections/{sectionId}/lessons/reorder")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ReorderLessons(Guid sectionId, [FromBody] ReorderLessonsRequest request)
    {
        var result = await _lessonService.ReorderLessonsAsync(sectionId, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
