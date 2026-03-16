using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/subscriptions")]
[Authorize]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _service;

    public SubscriptionController(ISubscriptionService service)
    {
        _service = service;
    }

    // ─── Helper ──────────────────────────────────────────────────────────────

    private Guid? GetCurrentUserId()
    {
        var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(value, out var id) ? id : null;
    }

    // ─── Student endpoints ───────────────────────────────────────────────────

    /// <summary>
    /// Get the current student's active subscription
    /// </summary>
    [HttpGet("me")]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(ServiceResult<SubscriptionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<SubscriptionDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMySubscription()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var result = await _service.GetCurrentSubscriptionAsync(userId.Value);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Subscribe to a package — creates UserSubscription (Pending) + Payment (Pending) and returns payment_url
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(ServiceResult<CreateSubscriptionResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResult<CreateSubscriptionResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Subscribe([FromBody] CreateSubscriptionRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var result = await _service.CreateSubscriptionAsync(userId.Value, request);
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }

    /// <summary>
    /// Renew a subscription — creates a new subscription starting from the previous end date and returns payment_url
    /// </summary>
    [HttpPost("{id:guid}/renew")]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(ServiceResult<CreateSubscriptionResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResult<CreateSubscriptionResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<CreateSubscriptionResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Renew(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var result = await _service.RenewSubscriptionAsync(userId.Value, id);

        if (!result.Success)
        {
            return result.Status == 404 ? NotFound(result) : BadRequest(result);
        }

        return StatusCode(201, result);
    }

    /// <summary>
    /// Cancel a subscription
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var result = await _service.CancelSubscriptionAsync(userId.Value, id);

        if (!result.Success)
        {
            return result.Status == 404 ? NotFound(result) : BadRequest(result);
        }

        return Ok(result);
    }

    // ─── Admin endpoints ─────────────────────────────────────────────────────

    /// <summary>
    /// Get all subscriptions (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<List<SubscriptionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllSubscriptionsAsync();
        return Ok(result);
    }
}
