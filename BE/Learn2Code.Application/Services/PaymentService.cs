using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Learn2Code.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learn2Code.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPayOsService _payOsService;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        IUnitOfWork unitOfWork,
        IPayOsService payOsService,
        ILogger<PaymentService> logger)
    {
        _unitOfWork = unitOfWork;
        _payOsService = payOsService;
        _logger = logger;
    }

    public async Task<ServiceResult<PayOsWebhookResponse>> ProcessPayOsWebhookAsync(PayOsWebhookRequest webhook, string rawBody)
    {
        try
        {
            // 1. Validate webhook data
            if (webhook.Data == null)
            {
                _logger.LogWarning("PayOS webhook received with null data");
                return ServiceResult<PayOsWebhookResponse>.BadRequest("Invalid webhook data");
            }

            // 2. Verify signature (recommended for security)
            if (!string.IsNullOrEmpty(webhook.Signature))
            {
                var isValid = _payOsService.VerifyWebhookSignature(rawBody, webhook.Signature);
                if (!isValid)
                {
                    _logger.LogWarning("PayOS webhook signature verification failed");
                    return ServiceResult<PayOsWebhookResponse>.Error("INVALID_SIGNATURE", "Webhook signature verification failed", 401);
                }
            }

            // 3. Find payment by orderCode (stored in TransactionId)
            var orderCode = webhook.Data.OrderCode.ToString();
            var payment = await _unitOfWork.Repository<Payment>()
                .GetAsync(p => p.TransactionId == orderCode);

            if (payment == null)
            {
                _logger.LogWarning("Payment not found for orderCode: {OrderCode}", orderCode);
                return ServiceResult<PayOsWebhookResponse>.NotFound("Payment not found");
            }

            // 4. Verify amount to prevent tampering
            var expectedAmount = (int)payment.Amount;
            if (webhook.Data.Amount != expectedAmount)
            {
                _logger.LogError("Amount mismatch for orderCode {OrderCode}: expected {Expected}, got {Actual}",
                    orderCode, expectedAmount, webhook.Data.Amount);
                return ServiceResult<PayOsWebhookResponse>.Error("AMOUNT_MISMATCH", "Payment amount mismatch", 400);
            }

            // 5. Determine new payment status
            var newStatus = DeterminePaymentStatus(webhook.Code, webhook.Data.Code);
            var oldStatus = payment.Status;

            // Skip if already processed
            if (oldStatus == newStatus)
            {
                _logger.LogInformation("Payment {PaymentId} already in status {Status}, skipping", payment.PaymentId, newStatus);
                return ServiceResult<PayOsWebhookResponse>.Ok(new PayOsWebhookResponse
                {
                    Success = true,
                    Message = "Payment already processed",
                    NewStatus = newStatus.ToString()
                });
            }

            // 6. Update payment status
            payment.Status = newStatus;
            if (newStatus == PaymentStatus.Success)
            {
                payment.PaidAt = DateTime.UtcNow;
            }

            _unitOfWork.Repository<Payment>().PrepareUpdate(payment);

            // 7. Auto-activate subscription if payment succeeded
            if (newStatus == PaymentStatus.Success)
            {
                await ActivateSubscriptionAsync(payment.SubscriptionId);
            }

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Payment {PaymentId} status updated: {OldStatus} → {NewStatus}",
                payment.PaymentId, oldStatus, newStatus);

            return ServiceResult<PayOsWebhookResponse>.Ok(new PayOsWebhookResponse
            {
                Success = true,
                Message = "Payment processed successfully",
                NewStatus = newStatus.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing PayOS webhook");
            return ServiceResult<PayOsWebhookResponse>.Error("PROCESSING_ERROR", "Error processing webhook", 500);
        }
    }

    public async Task<ServiceResult<List<PaymentDto>>> GetMyPaymentsAsync(Guid studentId)
    {
        var payments = await _unitOfWork.Repository<Payment>()
            .GetAllQueryable()
            .Include(p => p.Subscription)
            .Where(p => p.Subscription.UserId == studentId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return ServiceResult<List<PaymentDto>>.Ok(payments.ToPaymentDtoList());
    }

    public async Task<ServiceResult<List<PaymentDto>>> GetAllPaymentsAsync()
    {
        var payments = await _unitOfWork.Repository<Payment>()
            .GetAllQueryable()
            .Include(p => p.Subscription)
                .ThenInclude(s => s.User)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return ServiceResult<List<PaymentDto>>.Ok(payments.ToPaymentDtoList());
    }

    public async Task<ServiceResult> ConfirmBankTransferAsync(Guid paymentId)
    {
        var payment = await _unitOfWork.Repository<Payment>()
            .GetByIdAsync(paymentId);

        if (payment == null)
        {
            return ServiceResult.NotFound("Payment not found");
        }

        if (payment.Status == PaymentStatus.Success)
        {
            return ServiceResult.Error("ALREADY_CONFIRMED", "Payment already confirmed", 400);
        }

        payment.Status = PaymentStatus.Success;
        payment.PaidAt = DateTime.UtcNow;

        _unitOfWork.Repository<Payment>().PrepareUpdate(payment);

        // Activate subscription
        await ActivateSubscriptionAsync(payment.SubscriptionId);

        await _unitOfWork.CommitTransactionAsync();

        _logger.LogInformation("Bank transfer payment {PaymentId} confirmed by admin", paymentId);

        return ServiceResult.Ok("Payment confirmed and subscription activated");
    }

    // ─── Private Helpers ─────────────────────────────────────────────────────

    private PaymentStatus DeterminePaymentStatus(string webhookCode, string dataCode)
    {
        // PayOS returns code "00" for success
        if (webhookCode == "00" && dataCode == "00")
        {
            return PaymentStatus.Success;
        }

        return PaymentStatus.Failed;
    }

    private async Task ActivateSubscriptionAsync(Guid subscriptionId)
    {
        var subscription = await _unitOfWork.Repository<UserSubscription>()
            .GetByIdAsync(subscriptionId);

        if (subscription == null)
        {
            _logger.LogWarning("Subscription {SubscriptionId} not found for activation", subscriptionId);
            return;
        }

        if (subscription.Status == SubscriptionStatus.Active)
        {
            _logger.LogInformation("Subscription {SubscriptionId} already active", subscriptionId);
            return;
        }

        subscription.Status = SubscriptionStatus.Active;
        subscription.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<UserSubscription>().PrepareUpdate(subscription);

        _logger.LogInformation("Subscription {SubscriptionId} activated", subscriptionId);
    }
}
