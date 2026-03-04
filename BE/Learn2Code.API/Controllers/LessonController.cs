using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/lessons")]
[Authorize(Roles = "Student")]
public class LessonController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    /// <summary>
    /// Get lesson detail with access control checks
    /// </summary>
    [HttpGet("{lessonId}")]
    public async Task<IActionResult> GetLesson(Guid lessonId)
    {
        var studentId = GetCurrentUserId();
        var result = await _lessonService.GetLessonAsync(lessonId, studentId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    #region Helper Methods

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found"));
    }

    #endregion
}
