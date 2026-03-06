using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/courses/{courseId}/sections")]
public class SectionController : ControllerBase
{
    private readonly ISectionService _sectionService;

    public SectionController(ISectionService sectionService)
    {
        _sectionService = sectionService;
    }

    /// <summary>
    /// Get all sections of a course (Public)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ServiceResult<List<SectionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<List<SectionDto>>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(Guid courseId)
    {
        var result = await _sectionService.GetSectionsByCourseIdAsync(courseId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Get section by ID (Public)
    /// </summary>
    [HttpGet("{sectionId}")]
    [ProducesResponseType(typeof(ServiceResult<SectionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<SectionDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid courseId, Guid sectionId)
    {
        var result = await _sectionService.GetSectionByIdAsync(sectionId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Create a new section in a course (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<SectionDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResult<SectionDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<SectionDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(Guid courseId, [FromBody] CreateSectionRequest request)
    {
        var result = await _sectionService.CreateSectionAsync(courseId, request);
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }

    /// <summary>
    /// Update a section (Admin only)
    /// </summary>
    [HttpPatch("{sectionId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<SectionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<SectionDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<SectionDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(Guid courseId, Guid sectionId, [FromBody] UpdateSectionRequest request)
    {
        var result = await _sectionService.UpdateSectionAsync(courseId, sectionId, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Soft delete a section (Admin only)
    /// </summary>
    [HttpDelete("{sectionId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid courseId, Guid sectionId)
    {
        var result = await _sectionService.DeleteSectionAsync(courseId, sectionId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Reorder sections in a course (Admin only)
    /// </summary>
    [HttpPatch("reorder")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<List<SectionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<List<SectionDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<List<SectionDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Reorder(Guid courseId, [FromBody] ReorderSectionsRequest request)
    {
        var result = await _sectionService.ReorderSectionsAsync(courseId, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
