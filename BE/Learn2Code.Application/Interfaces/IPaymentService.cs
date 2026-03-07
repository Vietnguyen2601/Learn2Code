using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;

namespace Learn2Code.Application.Interfaces;

public interface IPaymentService
{
    /// <summary>
    /// Process PayOS webhook callback - updates Payment and UserSubscription status
    /// </summary>
    Task<ServiceResult<PayOsWebhookResponse>> ProcessPayOsWebhookAsync(PayOsWebhookRequest webhook, string rawBody);

    /// <summary>
    /// Get payment history for a specific student
    /// </summary>
    Task<ServiceResult<List<PaymentDto>>> GetMyPaymentsAsync(Guid studentId);

    /// <summary>
    /// Get all payments (Admin only)
    /// </summary>
    Task<ServiceResult<List<PaymentDto>>> GetAllPaymentsAsync();

    /// <summary>
    /// Verify payment from PayOS return URL and update status
    /// </summary>
    Task<ServiceResult> VerifyAndUpdatePaymentAsync(string orderCode, string status, string code, bool cancel);
}
