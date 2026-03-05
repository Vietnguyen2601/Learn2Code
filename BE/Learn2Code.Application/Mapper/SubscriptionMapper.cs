using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Application.Mapper;

public static class SubscriptionMapper
{
    // ─── Entity factories ─────────────────────────────────────────────────────

    public static UserSubscription ToNewUserSubscription(this SubscriptionPackage package, Guid userId, DateTime now)
    {
        return new UserSubscription
        {
            SubscriptionId = Guid.NewGuid(),
            UserId         = userId,
            PackageId      = package.PackageId,
            StartDate      = now,
            EndDate        = now.AddMonths(package.DurationMonths),
            Status         = SubscriptionStatus.Pending,
            CreatedAt      = now,
            UpdatedAt      = now
        };
    }

    public static UserSubscription ToRenewalUserSubscription(
        this SubscriptionPackage package, Guid userId, DateTime newStart, Guid renewedFromId, DateTime now)
    {
        return new UserSubscription
        {
            SubscriptionId = Guid.NewGuid(),
            UserId         = userId,
            PackageId      = package.PackageId,
            StartDate      = newStart,
            EndDate        = newStart.AddMonths(package.DurationMonths),
            Status         = SubscriptionStatus.Pending,
            RenewedFromId  = renewedFromId,
            CreatedAt      = now,
            UpdatedAt      = now
        };
    }

    /// <summary>Creates a Pending Payment for a subscription. Generates its own PaymentId and PayOS orderCode.</summary>
    public static Payment ToPendingPayment(this UserSubscription subscription, decimal amount)
    {
        var paymentId = Guid.NewGuid();
        return new Payment
        {
            PaymentId      = paymentId,
            SubscriptionId = subscription.SubscriptionId,
            Amount         = amount,
            PaymentMethod  = PaymentMethod.PayOS,
            TransactionId  = ToOrderCode(paymentId).ToString(),
            Status         = PaymentStatus.Pending,
            CreatedAt      = DateTime.UtcNow
        };
    }

    /// <summary>Reads the PayOS orderCode stored in TransactionId.</summary>
    public static long ToPayOSOrderCode(this Payment payment)
        => long.Parse(payment.TransactionId!);

    // ─── DTO factories ────────────────────────────────────────────────────────

    public static CreateSubscriptionResponse ToCreateSubscriptionResponse(
        this UserSubscription subscription, Payment payment, string packageName, string paymentUrl)
    {
        return new CreateSubscriptionResponse
        {
            SubscriptionId = subscription.SubscriptionId,
            PaymentId      = payment.PaymentId,
            PackageName    = packageName,
            Amount         = payment.Amount,
            PaymentMethod  = "PayOS",
            Status         = subscription.Status.ToString(),
            PaymentUrl     = paymentUrl,
            ExpiredAt      = subscription.EndDate
        };
    }

    public static SubscriptionDto ToSubscriptionDto(this UserSubscription subscription, SubscriptionPackage? package = null)
    {
        var pkg = subscription.Package ?? package;
        return new SubscriptionDto
        {
            SubscriptionId = subscription.SubscriptionId,
            UserId         = subscription.UserId,
            StartDate      = subscription.StartDate,
            EndDate        = subscription.EndDate,
            Status         = subscription.Status.ToString(),
            RenewedFromId  = subscription.RenewedFromId,
            CreatedAt      = subscription.CreatedAt,
            UpdatedAt      = subscription.UpdatedAt,
            Package        = pkg?.ToSubscriptionPackageDto()
        };
    }

    public static List<SubscriptionDto> ToSubscriptionDtoList(this IEnumerable<UserSubscription> subscriptions)
        => subscriptions.Select(s => s.ToSubscriptionDto()).ToList();

    // ─── Private helpers ──────────────────────────────────────────────────────

    /// <summary>Derives a unique positive long from a GUID for use as PayOS orderCode.</summary>
    private static long ToOrderCode(Guid id)
    {
        var raw = BitConverter.ToInt64(id.ToByteArray(), 0);
        return Math.Abs(raw == long.MinValue ? 1L : raw);
    }
}
