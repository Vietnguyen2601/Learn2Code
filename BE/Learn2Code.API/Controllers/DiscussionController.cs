using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.DiscussionDTOs.DiscussionRequests;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/lessons/{lessonId}/discussions")]
public class DiscussionController : ControllerBase
{
    private readonly IDiscussionService _discussionService;

    public DiscussionController(IDiscussionService discussionService)
    {
        _discussionService = discussionService;
    }

    /// <summary>
    /// L?y t?t c? discussions c?a m?t lesson
    /// </summary>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDiscussionsByLesson(Guid lessonId)
    {
        var result = await _discussionService.GetDiscussionsByLessonIdAsync(lessonId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// L?y chi ti?t m?t discussion
    /// </summary>
    [AllowAnonymous]
    [HttpGet("{discussionId}")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDiscussionById(Guid lessonId, Guid discussionId)
    {
        // T?ng view count
        await _discussionService.IncreaseViewCountAsync(discussionId);

        var result = await _discussionService.GetDiscussionByIdAsync(discussionId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// T?o discussion m?i
    /// </summary>
    [AllowAnonymous]  // T?m th?i t?t authorization ?? test
    [HttpPost]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateDiscussion(Guid lessonId, [FromBody] CreateDiscussionRequest request)
    {
        // N?u không có userId, dùng user hardcoded cho development
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            // TODO: Ch? dùng trong development
            parsedUserId = Guid.Parse("a0200000-0000-0000-0000-000000000001");
        }

        var result = await _discussionService.CreateDiscussionAsync(lessonId, parsedUserId, request);

        if (!result.Success)
        {
            return result.Status switch
            {
                404 => NotFound(result),
                400 => BadRequest(result),
                500 => StatusCode(500, result),
                _ => BadRequest(result)
            };
        }

        return StatusCode(201, result);
    }

    /// <summary>
    /// C?p nh?t discussion
    /// </summary>
    [AllowAnonymous]  // T?m th?i t?t authorization ?? test
    [HttpPut("{discussionId}")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDiscussion(Guid lessonId, Guid discussionId, [FromBody] UpdateDiscussionRequest request)
    {
        // N?u không có userId, dùng user hardcoded cho development
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            // TODO: Ch? dùng trong development
            parsedUserId = Guid.Parse("a0200000-0000-0000-0000-000000000001");
        }

        var result = await _discussionService.UpdateDiscussionAsync(discussionId, parsedUserId, request);

        if (!result.Success)
        {
            return result.Status switch
            {
                403 => Forbid(),
                404 => NotFound(result),
                400 => BadRequest(result),
                500 => StatusCode(500, result),
                _ => BadRequest(result)
            };
        }

        return Ok(result);
    }

    /// <summary>
    /// Xóa discussion
    /// </summary>
    [AllowAnonymous]  // T?m th?i t?t authorization ?? test
    [HttpDelete("{discussionId}")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDiscussion(Guid lessonId, Guid discussionId)
    {
        // N?u không có userId, dùng user hardcoded cho development
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            // TODO: Ch? dùng trong development
            parsedUserId = Guid.Parse("a0200000-0000-0000-0000-000000000001");
        }

        var result = await _discussionService.DeleteDiscussionAsync(discussionId, parsedUserId);

        if (!result.Success)
        {
            return result.Status switch
            {
                403 => Forbid(),
                404 => NotFound(result),
                400 => BadRequest(result),
                500 => StatusCode(500, result),
                _ => BadRequest(result)
            };
        }

        return Ok(result);
    }
}
