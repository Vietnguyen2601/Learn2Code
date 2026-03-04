using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/sections")]
[Authorize(Roles = "Student")]
public class SectionQuizController : ControllerBase
{
    private readonly IQuizService _quizService;

    public SectionQuizController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    /// <summary>
    /// Get section quiz with all questions (check if unlocked)
    /// </summary>
    [HttpGet("{sectionId}/section-quiz")]
    public async Task<IActionResult> GetSectionQuiz(Guid sectionId)
    {
        var studentId = GetCurrentUserId();
        var result = await _quizService.GetSectionQuizAsync(sectionId, studentId);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        // If not unlocked, return 403 with message
        if (!result.Data!.IsUnlocked)
        {
            return StatusCode(403, new
            {
                success = false,
                status_code = 403,
                error_code = "SECTION_NOT_COMPLETED",
                message = result.Data.UnlockMessage
            });
        }

        return Ok(result);
    }

    /// <summary>
    /// Submit section quiz attempt
    /// </summary>
    [HttpPost("{sectionId}/section-quiz/attempt")]
    public async Task<IActionResult> SubmitSectionQuiz(
        Guid sectionId,
        [FromBody] SubmitSectionQuizRequest request)
    {
        var studentId = GetCurrentUserId();
        var result = await _quizService.SubmitSectionQuizAsync(sectionId, studentId, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get student's section quiz attempt history
    /// </summary>
    [HttpGet("{sectionId}/section-quiz/attempts/me")]
    public async Task<IActionResult> GetMyAttempts(Guid sectionId)
    {
        var studentId = GetCurrentUserId();
        var result = await _quizService.GetMyAttemptsAsync(sectionId, studentId);
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
