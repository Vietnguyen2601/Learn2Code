using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/exercises")]
[Authorize(Roles = "Student")]
public class ExerciseController : ControllerBase
{
    private readonly IExerciseService _exerciseService;

    public ExerciseController(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    /// <summary>
    /// Run code for FreeCode exercise (no grading)
    /// </summary>
    [HttpPost("{exerciseId}/run")]
    public async Task<IActionResult> RunCode(Guid exerciseId, [FromBody] RunCodeRequest request)
    {
        var studentId = GetCurrentUserId();
        var result = await _exerciseService.RunCodeAsync(exerciseId, studentId, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Submit code for GradedCode exercise (with test cases grading)
    /// </summary>
    [HttpPost("{exerciseId}/submit")]
    public async Task<IActionResult> SubmitCode(Guid exerciseId, [FromBody] SubmitCodeRequest request)
    {
        var studentId = GetCurrentUserId();
        var result = await _exerciseService.SubmitCodeAsync(exerciseId, studentId, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Update exercise progress (auto-save last_code, mark completed)
    /// </summary>
    [HttpPatch("{exerciseId}/progress")]
    public async Task<IActionResult> UpdateProgress(Guid exerciseId, [FromBody] UpdateProgressRequest request)
    {
        var studentId = GetCurrentUserId();
        var result = await _exerciseService.UpdateProgressAsync(exerciseId, studentId, request);
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
