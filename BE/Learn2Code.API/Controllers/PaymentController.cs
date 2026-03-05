using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    // ─── Helper ──────────────────────────────────────────────────────────────

    private Guid? GetCurrentUserId()
    {
        var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(value, out var id) ? id : null;
    }

    // ─── Public Webhook ──────────────────────────────────────────────────────

    /// <summary>
    /// PayOS webhook callback - processes payment status updates
    /// </summary>
    [HttpPost("callback/payos")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ServiceResult<PayOsWebhookResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<PayOsWebhookResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PayOsWebhook([FromBody] PayOsWebhookRequest webhook)
    {
        // Read raw body for signature verification
        Request.Body.Position = 0;
        using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
        var rawBody = await reader.ReadToEndAsync();

        _logger.LogInformation("PayOS webhook received for orderCode: {OrderCode}", webhook.Data?.OrderCode);

        var result = await _paymentService.ProcessPayOsWebhookAsync(webhook, rawBody);

        return result.Success ? Ok(result) : BadRequest(result);
    }

    // ─── Admin endpoints ─────────────────────────────────────────────────────

    /// <summary>
    /// Get all payments (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<List<PaymentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _paymentService.GetAllPaymentsAsync();
        return Ok(result);
    }

    // ─── Student endpoints ───────────────────────────────────────────────────

    /// <summary>
    /// Get payment history of current student
    /// </summary>
    [HttpGet("me")]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(ServiceResult<List<PaymentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMy()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        var result = await _paymentService.GetMyPaymentsAsync(userId.Value);
        return Ok(result);
    }
}
