using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api")]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;

    public QuizController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    /// <summary>
    /// Get all quizzes in a lesson (Admin & Student)
    /// </summary>
    [HttpGet("lessons/{lessonId}/quizzes")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResult<List<QuizDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<List<QuizDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetQuizzesByLesson(Guid lessonId)
    {
        var result = await _quizService.GetQuizzesByLessonIdAsync(lessonId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Create a new quiz in lesson (Admin only)
    /// </summary>
    [HttpPost("lessons/{lessonId}/quizzes")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<QuizDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResult<QuizDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<QuizDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateQuiz(Guid lessonId, [FromBody] CreateQuizRequest request)
    {
        var result = await _quizService.CreateQuizAsync(lessonId, request);
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }

    /// <summary>
    /// Update a quiz (Admin only)
    /// </summary>
    [HttpPatch("quizzes/{quizId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<QuizDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<QuizDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<QuizDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateQuiz(Guid quizId, [FromBody] UpdateQuizRequest request)
    {
        var result = await _quizService.UpdateQuizAsync(quizId, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Delete a quiz (Admin only)
    /// </summary>
    [HttpDelete("quizzes/{quizId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteQuiz(Guid quizId)
    {
        var result = await _quizService.DeleteQuizAsync(quizId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Update a quiz option (Admin only)
    /// </summary>
    [HttpPatch("quizzes/{quizId}/options/{optionId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<QuizOptionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<QuizOptionDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<QuizOptionDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateQuizOption(Guid quizId, Guid optionId, [FromBody] UpdateSingleQuizOptionRequest request)
    {
        var result = await _quizService.UpdateQuizOptionAsync(quizId, optionId, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Delete a quiz option (Admin only)
    /// </summary>
    [HttpDelete("quizzes/{quizId}/options/{optionId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteQuizOption(Guid quizId, Guid optionId)
    {
        var result = await _quizService.DeleteQuizOptionAsync(quizId, optionId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Student answers a quiz and gets immediate feedback (Student only)
    /// </summary>
    [HttpPost("quizzes/{quizId}/answer")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ServiceResult<AnswerQuizResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<AnswerQuizResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<AnswerQuizResultDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ServiceResult<AnswerQuizResultDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AnswerQuiz(Guid quizId, [FromBody] AnswerQuizRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!);
        var result = await _quizService.AnswerQuizAsync(quizId, userId, request);
        return result.Success ? Ok(result) : StatusCode(result.Status, result);
    }
}
