using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/subscription-packages")]
public class SubscriptionPackageController : ControllerBase
{
    private readonly ISubscriptionPackageService _service;

    public SubscriptionPackageController(ISubscriptionPackageService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all active subscription packages (public — pricing page)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ServiceResult<List<SubscriptionPackageDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    /// <summary>
    /// Get a specific subscription package by ID (public)
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ServiceResult<SubscriptionPackageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<SubscriptionPackageDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Create a new subscription package (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<SubscriptionPackageDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResult<SubscriptionPackageDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSubscriptionPackageRequest request)
    {
        var result = await _service.CreateAsync(request);
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }

    /// <summary>
    /// Update a subscription package (Admin only)
    /// </summary>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<SubscriptionPackageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<SubscriptionPackageDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<SubscriptionPackageDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubscriptionPackageRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        return result.Success ? Ok(result) : result.Status == 404 ? NotFound(result) : BadRequest(result);
    }

    /// <summary>
    /// Disable (soft-delete) a subscription package (Admin only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Disable(Guid id)
    {
        var result = await _service.DisableAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
