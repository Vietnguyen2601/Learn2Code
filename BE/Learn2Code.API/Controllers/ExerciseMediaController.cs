using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.ExerciseMediaDTOs.ExerciseMediaRequests;
using Learn2Code.Application.DTOs.ExerciseMediaDTOs.ExerciseMediaResponses;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/exercises")]
public class ExerciseMediaController : ControllerBase
{
    private readonly IExerciseMediaService _exerciseMediaService;

    public ExerciseMediaController(IExerciseMediaService exerciseMediaService)
    {
        _exerciseMediaService = exerciseMediaService;
    }

    /// <summary>
    /// Get all media of an exercise
    /// </summary>
    [HttpGet("{exerciseId}/media")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResult<List<MediaDetailDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<List<MediaDetailDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMediaByExercise(Guid exerciseId)
    {
        var result = await _exerciseMediaService.GetMediaByExerciseIdAsync(exerciseId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Upload media for a Reading exercise (Admin only)
    /// </summary>
    [HttpPost("{exerciseId}/media")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<MediaDetailDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResult<MediaDetailDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<MediaDetailDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateMedia(Guid exerciseId, [FromBody] CreateExerciseMediaRequest request)
    {
        var result = await _exerciseMediaService.CreateMediaAsync(exerciseId, request);
        if (result.Success)
            return StatusCode(201, result);
        return result.Status == 404 ? NotFound(result) : BadRequest(result);
    }

    /// <summary>
    /// Delete a media item (Admin only)
    /// </summary>
    [HttpDelete("{exerciseId}/media/{mediaId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteMedia(Guid exerciseId, Guid mediaId)
    {
        var result = await _exerciseMediaService.DeleteMediaAsync(exerciseId, mediaId);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
