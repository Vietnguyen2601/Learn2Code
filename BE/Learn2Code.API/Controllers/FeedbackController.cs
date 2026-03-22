using System.Security.Claims;
using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.FeedbackDTOs.FeedbackRequests;
using Learn2Code.Application.DTOs.FeedbackDTOs.FeedbackResponses;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/courses/{courseId}/feedbacks")]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackService _feedbackService;

    public FeedbackController(IFeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    /// <summary>
    /// Get all feedbacks for a course (public)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ServiceResult<List<FeedbackDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<List<FeedbackDto>>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFeedbacks(Guid courseId)
    {
        var result = await _feedbackService.GetFeedbacksByCourseAsync(courseId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Submit feedback for a course (Student only, must be enrolled)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ServiceResult<FeedbackDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResult<FeedbackDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateFeedback(Guid courseId, [FromBody] CreateFeedbackRequest request)
    {
        var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _feedbackService.CreateFeedbackAsync(courseId, studentId, request);
        if (result.Success)
            return StatusCode(201, result);
        return result.Status == 404 ? NotFound(result) : BadRequest(result);
    }

    /// <summary>
    /// Update my feedback (Student only)
    /// </summary>
    [HttpPatch("me")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ServiceResult<FeedbackDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<FeedbackDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateMyFeedback(Guid courseId, [FromBody] UpdateFeedbackRequest request)
    {
        var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _feedbackService.UpdateMyFeedbackAsync(courseId, studentId, request);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Delete my feedback (Student only)
    /// </summary>
    [HttpDelete("me")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteMyFeedback(Guid courseId)
    {
        var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _feedbackService.DeleteMyFeedbackAsync(courseId, studentId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Admin delete feedback (Admin only)
    /// </summary>
    [HttpDelete("{feedbackId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AdminDeleteFeedback(Guid courseId, Guid feedbackId)
    {
        var result = await _feedbackService.AdminDeleteFeedbackAsync(courseId, feedbackId);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
