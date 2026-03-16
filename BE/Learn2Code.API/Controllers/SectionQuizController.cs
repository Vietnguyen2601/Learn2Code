using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/sections")]
public class SectionQuizController : ControllerBase
{
    private readonly ISectionQuizService _sectionQuizService;

    public SectionQuizController(ISectionQuizService sectionQuizService)
    {
        _sectionQuizService = sectionQuizService;
    }

    /// <summary>
    /// Get all quizzes in a section for quiz exam (Student only)
    /// Requires all lessons in section to be completed
    /// </summary>
    [HttpGet("{sectionId}/section-quiz")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ServiceResult<SectionQuizDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<SectionQuizDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResult<SectionQuizDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSectionQuiz(Guid sectionId)
    {
        var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _sectionQuizService.GetSectionQuizAsync(sectionId, studentId);
        return result.Success ? Ok(result) : StatusCode(result.Status, result);
    }

    /// <summary>
    /// Submit section quiz attempt (Student only)
    /// </summary>
    [HttpPost("{sectionId}/section-quiz/attempt")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ServiceResult<SectionQuizAttemptResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<SectionQuizAttemptResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<SectionQuizAttemptResultDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SubmitSectionQuizAttempt(Guid sectionId, [FromBody] SubmitSectionQuizRequest request)
    {
        var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _sectionQuizService.SubmitSectionQuizAttemptAsync(sectionId, studentId, request);
        return result.Success ? Ok(result) : StatusCode(result.Status, result);
    }

    /// <summary>
    /// Get student's attempt history for a section quiz (Student only)
    /// </summary>
    [HttpGet("{sectionId}/section-quiz/attempts/me")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ServiceResult<List<SectionQuizAttemptResultDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<List<SectionQuizAttemptResultDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyAttempts(Guid sectionId)
    {
        var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _sectionQuizService.GetStudentAttemptsAsync(sectionId, studentId);
        return result.Success ? Ok(result) : StatusCode(result.Status, result);
    }
}
