using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/quizzes")]
[Authorize(Roles = "Student")]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;

    public QuizController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    /// <summary>
    /// Answer a single quiz in a lesson
    /// </summary>
    [HttpPost("{quizId}/answer")]
    public async Task<IActionResult> AnswerQuiz(Guid quizId, [FromBody] AnswerQuizRequest request)
    {
        var studentId = GetCurrentUserId();
        var result = await _quizService.AnswerQuizAsync(quizId, studentId, request);
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
