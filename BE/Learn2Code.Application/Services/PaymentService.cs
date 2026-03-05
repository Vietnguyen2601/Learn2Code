using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Learn2Code.Infrastructure.Services;
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
        var payments = await _unitOfWork.PaymentRepository.GetByStudentIdAsync(studentId);
        return ServiceResult<List<PaymentDto>>.Ok(payments.ToPaymentDtoList());
    }

    public async Task<ServiceResult<List<PaymentDto>>> GetAllPaymentsAsync()
    {
        var payments = await _unitOfWork.PaymentRepository.GetAllWithDetailsAsync();
        return ServiceResult<List<PaymentDto>>.Ok(payments.ToPaymentDtoList());
    }

    public async Task<ServiceResult> VerifyAndUpdatePaymentAsync(string orderCode, string status, string code, bool cancel)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // 1. Find payment by orderCode
            var payment = await _unitOfWork.Repository<Payment>()
                .GetAsync(p => p.TransactionId == orderCode);

            if (payment == null)
            {
                _logger.LogWarning("Payment not found for orderCode: {OrderCode}", orderCode);
                return ServiceResult.NotFound("Payment not found");
            }

            // 2. Handle cancelled payment
            if (cancel || status == "CANCELLED")
            {
                if (payment.Status == PaymentStatus.Pending)
                {
                    payment.Status = PaymentStatus.Failed;
                    _unitOfWork.Repository<Payment>().PrepareUpdate(payment);
                    await _unitOfWork.CommitTransactionAsync();
                    _logger.LogInformation("Payment {PaymentId} marked as Failed (cancelled by user)", payment.PaymentId);
                }
                return ServiceResult.BadRequest("Payment was cancelled");
            }

            // 3. Skip if already processed
            if (payment.Status == PaymentStatus.Success)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult.Ok("Payment already confirmed");
            }

            // 4. Validate PayOS code and status
            if (code != "00" || status != "PAID")
            {
                _logger.LogWarning("Payment return with non-success: code={Code}, status={Status}", code, status);
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult.BadRequest($"Payment not successful: status={status}");
            }

            // 5. Verify with PayOS API
            var payOsPayment = await _payOsService.GetPaymentInfoAsync(long.Parse(orderCode));

            if (payOsPayment == null)
            {
                _logger.LogError("Cannot verify payment from PayOS for orderCode: {OrderCode}", orderCode);
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult.Error("PAYOS_ERROR", "Cannot verify payment from PayOS", 500);
            }

            // 6. Verify amount matches
            if (payOsPayment.Amount != (int)payment.Amount)
            {
                _logger.LogError("Amount mismatch: expected {Expected}, got {Actual}", payment.Amount, payOsPayment.Amount);
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult.Error("AMOUNT_MISMATCH", "Payment amount mismatch", 400);
            }

            // 7. Verify PayOS status is PAID
            if (payOsPayment.Status?.ToUpper() != "PAID")
            {
                _logger.LogWarning("PayOS status not PAID: {Status}", payOsPayment.Status);
                await _unitOfWork.RollbackTransactionAsync();
                return ServiceResult.BadRequest($"PayOS payment status: {payOsPayment.Status}");
            }

            // 8. Update payment to Success
            payment.Status = PaymentStatus.Success;
            payment.PaidAt = DateTime.UtcNow;
            _unitOfWork.Repository<Payment>().PrepareUpdate(payment);

            // 9. Activate subscription
            await ActivateSubscriptionAsync(payment.SubscriptionId);

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Payment {PaymentId} verified and subscription activated via return URL",
                payment.PaymentId);

            return ServiceResult.Ok("Payment confirmed and subscription activated");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error verifying payment for orderCode {OrderCode}", orderCode);
            return ServiceResult.Error("VERIFY_FAILED", "Failed to verify payment", 500);
        }
    }

    // ─── Private Helpers ─────────────────────────────────────────────────────

    private static PaymentStatus DeterminePaymentStatus(string webhookCode, string dataCode)
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
