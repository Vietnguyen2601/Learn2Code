using System.Security.Claims;
using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api")]
public class ExerciseController : ControllerBase
{
    private readonly IExerciseService _exerciseService;

    public ExerciseController(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    /// <summary>
    /// Get all exercises in a lesson (Admin & Student)
    /// </summary>
    [HttpGet("lessons/{lessonId}/exercises")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResult<List<ExerciseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<List<ExerciseDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExercisesByLesson(Guid lessonId)
    {
        var result = await _exerciseService.GetExercisesByLessonIdAsync(lessonId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Get exercise detail by ID (Admin & Student - with access control)
    /// </summary>
    [HttpGet("exercises/{exerciseId}")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResult<ExerciseDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<ExerciseDetailDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResult<ExerciseDetailDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetExerciseById(Guid exerciseId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Guid? userId = userIdClaim != null ? Guid.Parse(userIdClaim) : null;

        var result = await _exerciseService.GetExerciseByIdAsync(exerciseId, userId);
        
        if (!result.Success)
        {
            return result.Status == 403 ? StatusCode(403, result) : NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Create a new exercise in lesson (Admin only)
    /// </summary>
    [HttpPost("lessons/{lessonId}/exercises")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<ExerciseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResult<ExerciseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<ExerciseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateExercise(Guid lessonId, [FromBody] CreateExerciseRequest request)
    {
        var result = await _exerciseService.CreateExerciseAsync(lessonId, request);
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }

    /// <summary>
    /// Update an exercise (Admin only)
    /// </summary>
    [HttpPatch("exercises/{exerciseId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<ExerciseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<ExerciseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<ExerciseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateExercise(Guid exerciseId, [FromBody] UpdateExerciseRequest request)
    {
        var result = await _exerciseService.UpdateExerciseAsync(exerciseId, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Delete an exercise (Admin only)
    /// </summary>
    [HttpDelete("exercises/{exerciseId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteExercise(Guid exerciseId)
    {
        var result = await _exerciseService.DeleteExerciseAsync(exerciseId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Run code for an exercise (saves last code, no judge) (Student)
    /// </summary>
    [HttpPost("exercises/{exerciseId}/run")]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(ServiceResult<ExerciseProgressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<ExerciseProgressDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResult<ExerciseProgressDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RunCode(Guid exerciseId, [FromBody] RunCodeRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _exerciseService.RunCodeAsync(exerciseId, userId, request);

        if (!result.Success)
            return result.Status == 403 ? StatusCode(403, result) : NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Submit code for an exercise (marks as completed) (Student)
    /// </summary>
    [HttpPost("exercises/{exerciseId}/submit")]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(ServiceResult<ExerciseProgressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<ExerciseProgressDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResult<ExerciseProgressDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SubmitCode(Guid exerciseId, [FromBody] SubmitCodeRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _exerciseService.SubmitCodeAsync(exerciseId, userId, request);

        if (!result.Success)
            return result.Status == 403 ? StatusCode(403, result) : NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Update exercise progress manually (for Reading exercises) (Student)
    /// </summary>
    [HttpPatch("exercises/{exerciseId}/progress")]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(ServiceResult<ExerciseProgressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<ExerciseProgressDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResult<ExerciseProgressDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProgress(Guid exerciseId, [FromBody] UpdateExerciseProgressRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _exerciseService.UpdateExerciseProgressAsync(exerciseId, userId, request);

        if (!result.Success)
            return result.Status == 403 ? StatusCode(403, result) : NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Get current student's progress for an exercise (Student)
    /// </summary>
    [HttpGet("exercises/{exerciseId}/progress")]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(ServiceResult<ExerciseProgressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<ExerciseProgressDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProgress(Guid exerciseId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _exerciseService.GetExerciseProgressAsync(exerciseId, userId);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
