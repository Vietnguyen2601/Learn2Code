using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;

namespace Learn2Code.Application.Mapper;

public static class PaymentMapper
{
    public static PaymentDto ToPaymentDto(this Payment payment)
    {
        return new PaymentDto
        {
            PaymentId = payment.PaymentId,
            SubscriptionId = payment.SubscriptionId,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod.ToString(),
            TransactionId = payment.TransactionId,
            Status = payment.Status.ToString(),
            PaidAt = payment.PaidAt,
            CreatedAt = payment.CreatedAt
        };
    }

    public static List<PaymentDto> ToPaymentDtoList(this IEnumerable<Payment> payments)
    {
        return payments.Select(p => p.ToPaymentDto()).ToList();
    }
}
