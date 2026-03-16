using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/courses")]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    /// <summary>
    /// Get all active courses with optional filters (Public)
    /// </summary>
    /// <param name="category_id">Filter by category ID</param>
    /// <param name="difficulty">Filter by difficulty (Beginner, Intermediate, Advanced)</param>
    /// <param name="search">Search by title or description</param>
    [HttpGet]
    [ProducesResponseType(typeof(ServiceResult<List<CourseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery(Name = "category_id")] Guid? category_id = null,
        [FromQuery(Name = "difficulty")] string? difficulty = null,
        [FromQuery(Name = "search")] string? search = null)
    {
        CourseDifficulty? parsedDifficulty = null;
        if (!string.IsNullOrWhiteSpace(difficulty) && 
            Enum.TryParse<CourseDifficulty>(difficulty, true, out var tempDifficulty))
        {
            parsedDifficulty = tempDifficulty;
        }

        var result = await _courseService.GetAllCoursesAsync(category_id, parsedDifficulty, search);
        return Ok(result);
    }

    /// <summary>
    /// Get all active courses (Admin only)
    /// </summary>
    [HttpGet("active")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<List<CourseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetActive(
        [FromQuery(Name = "category_id")] Guid? category_id = null,
        [FromQuery(Name = "difficulty")] string? difficulty = null,
        [FromQuery(Name = "search")] string? search = null)
    {
        CourseDifficulty? parsedDifficulty = null;
        if (!string.IsNullOrWhiteSpace(difficulty) && 
            Enum.TryParse<CourseDifficulty>(difficulty, true, out var tempDifficulty))
        {
            parsedDifficulty = tempDifficulty;
        }

        var result = await _courseService.GetActiveCoursesAsync(category_id, parsedDifficulty, search);
        return Ok(result);
    }

    /// <summary>
    /// Get all inactive/deleted courses (Admin only)
    /// </summary>
    [HttpGet("inactive")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<List<CourseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetInactive(
        [FromQuery(Name = "category_id")] Guid? category_id = null,
        [FromQuery(Name = "difficulty")] string? difficulty = null,
        [FromQuery(Name = "search")] string? search = null)
    {
        CourseDifficulty? parsedDifficulty = null;
        if (!string.IsNullOrWhiteSpace(difficulty) && 
            Enum.TryParse<CourseDifficulty>(difficulty, true, out var tempDifficulty))
        {
            parsedDifficulty = tempDifficulty;
        }

        var result = await _courseService.GetInactiveCoursesAsync(category_id, parsedDifficulty, search);
        return Ok(result);
    }

    /// <summary>
    /// Get course detail by ID with sections (Public)
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ServiceResult<CourseDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<CourseDetailDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _courseService.GetCourseDetailByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Create a new course (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<CourseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResult<CourseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest request)
    {
        var result = await _courseService.CreateCourseAsync(request);
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }

    /// <summary>
    /// Update an existing course (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<CourseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<CourseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourseRequest request)
    {
        var result = await _courseService.UpdateCourseAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Soft delete a course (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _courseService.DeleteCourseAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Restore a deleted course (Admin only)
    /// </summary>
    [HttpPatch("{id}/restore")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<CourseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<CourseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<CourseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Restore(Guid id)
    {
        var result = await _courseService.RestoreCourseAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
