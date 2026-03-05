using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Learn2Code.Application.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _returnUrl;
    private readonly string _cancelUrl;

    public SubscriptionService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _returnUrl  = configuration["PayOS:ReturnUrl"] ?? "https://localhost:5001/payment/success";
        _cancelUrl  = configuration["PayOS:CancelUrl"] ?? "https://localhost:5001/payment/cancel";
    }

    // [S] GET /subscriptions/me
    public async Task<ServiceResult<SubscriptionDto>> GetCurrentSubscriptionAsync(Guid userId)
    {
        var subscription = await _unitOfWork.Repository<UserSubscription>()
            .GetAllQueryable()
            .Include(s => s.Package)
            .Where(s => s.UserId == userId &&
                        (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Pending))
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        if (subscription == null)
            return ServiceResult<SubscriptionDto>.NotFound("No active subscription found");

        return ServiceResult<SubscriptionDto>.Ok(MapToDto(subscription));
    }

    // [S] POST /subscriptions
    public async Task<ServiceResult<CreateSubscriptionResponse>> CreateSubscriptionAsync(Guid userId, CreateSubscriptionRequest request)
    {
        // Validate package
        var package = await _unitOfWork.Repository<SubscriptionPackage>().GetByIdAsync(request.PackageId);
        if (package == null || !package.IsActive)
            return ServiceResult<CreateSubscriptionResponse>.Error("PACKAGE_NOT_FOUND", "Subscription package not found or inactive");

        // Block if already active
        var existingActive = await _unitOfWork.Repository<UserSubscription>()
            .AnyAsync(s => s.UserId == userId && s.Status == SubscriptionStatus.Active);
        if (existingActive)
            return ServiceResult<CreateSubscriptionResponse>.Error("ALREADY_SUBSCRIBED", "You already have an active subscription. Use renew or cancel first.");

        var now            = DateTime.UtcNow;
        var subscriptionId = Guid.NewGuid();
        var paymentId      = Guid.NewGuid();
        var effectivePrice = package.Price * (1 - package.DiscountPercent / 100m);
        var orderCode      = ToOrderCode(paymentId);

        var subscription = new UserSubscription
        {
            SubscriptionId = subscriptionId,
            UserId         = userId,
            PackageId      = request.PackageId,
            StartDate      = now,
            EndDate        = now.AddMonths(package.DurationMonths),
            Status         = SubscriptionStatus.Pending,
            CreatedAt      = now,
            UpdatedAt      = now
        };

        var payment = new Payment
        {
            PaymentId      = paymentId,
            SubscriptionId = subscriptionId,
            Amount         = effectivePrice,
            PaymentMethod  = PaymentMethod.PayOS,
            TransactionId  = orderCode.ToString(),
            Status         = PaymentStatus.Pending,
            CreatedAt      = now
        };

        _unitOfWork.Repository<UserSubscription>().PrepareCreate(subscription);
        _unitOfWork.Repository<Payment>().PrepareCreate(payment);
        await _unitOfWork.CommitTransactionAsync();

        // TODO: replace with real PayOS SDK call when integrated
        var checkoutUrl = $"{_returnUrl}?orderCode={orderCode}&amount={(int)effectivePrice}";

        return ServiceResult<CreateSubscriptionResponse>.Created(new CreateSubscriptionResponse
        {
            SubscriptionId = subscriptionId,
            PaymentId      = paymentId,
            PackageName    = package.Name,
            Amount         = effectivePrice,
            PaymentMethod  = "PayOS",
            Status         = SubscriptionStatus.Pending.ToString(),
            PaymentUrl     = checkoutUrl,
            ExpiredAt      = now.AddMonths(package.DurationMonths)
        });
    }

    // [S] POST /subscriptions/:id/renew
    public async Task<ServiceResult<CreateSubscriptionResponse>> RenewSubscriptionAsync(Guid userId, Guid subscriptionId)
    {
        var existing = await _unitOfWork.Repository<UserSubscription>()
            .GetAllQueryable()
            .Include(s => s.Package)
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId && s.UserId == userId);

        if (existing == null)
            return ServiceResult<CreateSubscriptionResponse>.NotFound("Subscription not found");

        if (existing.Status == SubscriptionStatus.Cancelled)
            return ServiceResult<CreateSubscriptionResponse>.Error("SUBSCRIPTION_CANCELLED", "Cannot renew a cancelled subscription");

        if (existing.Package == null || !existing.Package.IsActive)
            return ServiceResult<CreateSubscriptionResponse>.Error("PACKAGE_INACTIVE", "The subscription package is no longer available");

        var package        = existing.Package;
        var now            = DateTime.UtcNow;
        var newStart       = existing.EndDate > now ? existing.EndDate : now;
        var renewalId      = Guid.NewGuid();
        var paymentId      = Guid.NewGuid();
        var effectivePrice = package.Price * (1 - package.DiscountPercent / 100m);
        var orderCode      = ToOrderCode(paymentId);

        var renewal = new UserSubscription
        {
            SubscriptionId = renewalId,
            UserId         = userId,
            PackageId      = package.PackageId,
            StartDate      = newStart,
            EndDate        = newStart.AddMonths(package.DurationMonths),
            Status         = SubscriptionStatus.Pending,
            RenewedFromId  = subscriptionId,
            CreatedAt      = now,
            UpdatedAt      = now
        };

        var payment = new Payment
        {
            PaymentId      = paymentId,
            SubscriptionId = renewalId,
            Amount         = effectivePrice,
            PaymentMethod  = PaymentMethod.PayOS,
            TransactionId  = orderCode.ToString(),
            Status         = PaymentStatus.Pending,
            CreatedAt      = now
        };

        _unitOfWork.Repository<UserSubscription>().PrepareCreate(renewal);
        _unitOfWork.Repository<Payment>().PrepareCreate(payment);
        await _unitOfWork.CommitTransactionAsync();

        // TODO: replace with real PayOS SDK call when integrated
        var checkoutUrl = $"{_returnUrl}?orderCode={orderCode}&amount={(int)effectivePrice}";

        return ServiceResult<CreateSubscriptionResponse>.Created(new CreateSubscriptionResponse
        {
            SubscriptionId = renewalId,
            PaymentId      = paymentId,
            PackageName    = package.Name,
            Amount         = effectivePrice,
            PaymentMethod  = "PayOS",
            Status         = SubscriptionStatus.Pending.ToString(),
            PaymentUrl     = checkoutUrl,
            ExpiredAt      = newStart.AddMonths(package.DurationMonths)
        });
    }

    // [S] POST /subscriptions/:id/cancel
    public async Task<ServiceResult> CancelSubscriptionAsync(Guid userId, Guid subscriptionId)
    {
        var subscription = await _unitOfWork.Repository<UserSubscription>()
            .GetAsync(s => s.SubscriptionId == subscriptionId && s.UserId == userId);

        if (subscription == null)
            return ServiceResult.NotFound("Subscription not found");

        if (subscription.Status == SubscriptionStatus.Cancelled)
            return ServiceResult.Error("ALREADY_CANCELLED", "Subscription is already cancelled");

        if (subscription.Status == SubscriptionStatus.Expired)
            return ServiceResult.Error("ALREADY_EXPIRED", "Subscription is already expired");

        subscription.Status    = SubscriptionStatus.Cancelled;
        subscription.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<UserSubscription>().PrepareUpdate(subscription);
        await _unitOfWork.CommitTransactionAsync();

        return ServiceResult.Ok("Subscription has been cancelled");
    }

    // [A] GET /subscriptions
    public async Task<ServiceResult<List<SubscriptionDto>>> GetAllSubscriptionsAsync()
    {
        var subscriptions = await _unitOfWork.Repository<UserSubscription>()
            .GetAllQueryable()
            .Include(s => s.Package)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        var dtos = subscriptions.Select(s => MapToDto(s)).ToList();
        return ServiceResult<List<SubscriptionDto>>.Ok(dtos);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    /// <summary>Derive a unique positive long from a GUID (PayOS orderCode).</summary>
    private static long ToOrderCode(Guid id)
    {
        var raw = BitConverter.ToInt64(id.ToByteArray(), 0);
        return Math.Abs(raw == long.MinValue ? 1L : raw);
    }

    private static string Truncate(string s, int max) =>
        s.Length <= max ? s : s[..max];

    // ─── Mappers ─────────────────────────────────────────────────────────────

    private static SubscriptionDto MapToDto(UserSubscription s, SubscriptionPackage? package = null)
    {
        var pkg = s.Package ?? package;
        return new SubscriptionDto
        {
            SubscriptionId = s.SubscriptionId,
            UserId         = s.UserId,
            StartDate      = s.StartDate,
            EndDate        = s.EndDate,
            Status         = s.Status.ToString(),
            RenewedFromId  = s.RenewedFromId,
            CreatedAt      = s.CreatedAt,
            UpdatedAt      = s.UpdatedAt,
            Package        = pkg == null ? null : new SubscriptionPackageDto
            {
                PackageId       = pkg.PackageId,
                Name            = pkg.Name,
                DurationMonths  = pkg.DurationMonths,
                Price           = pkg.Price,
                DiscountPercent = pkg.DiscountPercent,
                Description     = pkg.Description,
                IsActive        = pkg.IsActive,
                CreatedAt       = pkg.CreatedAt,
                UpdatedAt       = pkg.UpdatedAt
            }
        };
    }

}
