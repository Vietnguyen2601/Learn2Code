using System.Security.Claims;
using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/enrollments")]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    /// <summary>
    /// Get current student's enrollments
    /// </summary>
    [HttpGet("me")]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(ServiceResult<List<EnrollmentDetailDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyEnrollments()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _enrollmentService.GetMyEnrollmentsAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Get enrollment by ID (owner or Admin)
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResult<EnrollmentDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<EnrollmentDetailDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResult<EnrollmentDetailDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetEnrollmentById(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");
        var result = await _enrollmentService.GetEnrollmentByIdAsync(id, userId, isAdmin);

        if (!result.Success)
            return result.Status == 403 ? StatusCode(403, result) : NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Enroll in a course (requires active subscription)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(ServiceResult<EnrollmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResult<EnrollmentDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<EnrollmentDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ServiceResult<EnrollmentDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateEnrollment([FromBody] CreateEnrollmentRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _enrollmentService.CreateEnrollmentAsync(userId, request);

        if (!result.Success)
            return result.Status == 403 ? StatusCode(403, result)
                : result.Status == 404 ? NotFound(result)
                : BadRequest(result);

        return StatusCode(201, result);
    }

    /// <summary>
    /// Get all enrollments (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<List<EnrollmentDetailDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllEnrollments()
    {
        var result = await _enrollmentService.GetAllEnrollmentsAsync();
        return Ok(result);
    }
}
