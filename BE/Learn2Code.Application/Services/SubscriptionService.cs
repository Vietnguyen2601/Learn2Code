using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
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
        var subscription = await _unitOfWork.SubscriptionRepository.GetCurrentActiveAsync(userId);

        if (subscription == null)
            return ServiceResult<SubscriptionDto>.NotFound("No active subscription found");

        return ServiceResult<SubscriptionDto>.Ok(subscription.ToSubscriptionDto());
    }

    // [S] POST /subscriptions
    public async Task<ServiceResult<CreateSubscriptionResponse>> CreateSubscriptionAsync(Guid userId, CreateSubscriptionRequest request)
    {
        // Validate package
        var package = await _unitOfWork.SubscriptionPackageRepository.GetByIdAsync(request.PackageId);
        if (package == null || !package.IsActive)
            return ServiceResult<CreateSubscriptionResponse>.Error("PACKAGE_NOT_FOUND", "Subscription package not found or inactive");

        // Block if already active
        var existingActive = await _unitOfWork.SubscriptionRepository
            .AnyAsync(s => s.UserId == userId && s.Status == SubscriptionStatus.Active);
        if (existingActive)
            return ServiceResult<CreateSubscriptionResponse>.Error("ALREADY_SUBSCRIBED", "You already have an active subscription. Use renew or cancel first.");

        var now            = DateTime.UtcNow;
        var effectivePrice = package.Price * (1 - package.DiscountPercent / 100m);
        var subscription   = package.ToNewUserSubscription(userId, now);
        var payment        = subscription.ToPendingPayment(effectivePrice);

        // TODO: replace with real PayOS SDK call when integrated
        var checkoutUrl = $"{_returnUrl}?orderCode={payment.ToPayOSOrderCode()}&amount={(int)effectivePrice}";

        _unitOfWork.SubscriptionRepository.PrepareCreate(subscription);
        _unitOfWork.Repository<Payment>().PrepareCreate(payment);
        await _unitOfWork.CommitTransactionAsync();

        return ServiceResult<CreateSubscriptionResponse>.Created(
            subscription.ToCreateSubscriptionResponse(payment, package.Name, checkoutUrl));
    }

    // [S] POST /subscriptions/:id/renew
    public async Task<ServiceResult<CreateSubscriptionResponse>> RenewSubscriptionAsync(Guid userId, Guid subscriptionId)
    {
        var existing = await _unitOfWork.SubscriptionRepository.GetByIdWithDetailsAsync(subscriptionId, userId);

        if (existing == null)
            return ServiceResult<CreateSubscriptionResponse>.NotFound("Subscription not found");

        if (existing.Status == SubscriptionStatus.Cancelled)
            return ServiceResult<CreateSubscriptionResponse>.Error("SUBSCRIPTION_CANCELLED", "Cannot renew a cancelled subscription");

        if (existing.Package == null || !existing.Package.IsActive)
            return ServiceResult<CreateSubscriptionResponse>.Error("PACKAGE_INACTIVE", "The subscription package is no longer available");

        var package        = existing.Package;
        var now            = DateTime.UtcNow;
        var newStart       = existing.EndDate > now ? existing.EndDate : now;
        var effectivePrice = package.Price * (1 - package.DiscountPercent / 100m);
        var renewal        = package.ToRenewalUserSubscription(userId, newStart, subscriptionId, now);
        var payment        = renewal.ToPendingPayment(effectivePrice);

        // TODO: replace with real PayOS SDK call when integrated
        var checkoutUrl = $"{_returnUrl}?orderCode={payment.ToPayOSOrderCode()}&amount={(int)effectivePrice}";

        _unitOfWork.SubscriptionRepository.PrepareCreate(renewal);
        _unitOfWork.Repository<Payment>().PrepareCreate(payment);
        await _unitOfWork.CommitTransactionAsync();

        return ServiceResult<CreateSubscriptionResponse>.Created(
            renewal.ToCreateSubscriptionResponse(payment, package.Name, checkoutUrl));
    }

    // [S] POST /subscriptions/:id/cancel
    public async Task<ServiceResult> CancelSubscriptionAsync(Guid userId, Guid subscriptionId)
    {
        var subscription = await _unitOfWork.SubscriptionRepository
            .GetAsync(s => s.SubscriptionId == subscriptionId && s.UserId == userId);

        if (subscription == null)
            return ServiceResult.NotFound("Subscription not found");

        if (subscription.Status == SubscriptionStatus.Cancelled)
            return ServiceResult.Error("ALREADY_CANCELLED", "Subscription is already cancelled");

        if (subscription.Status == SubscriptionStatus.Expired)
            return ServiceResult.Error("ALREADY_EXPIRED", "Subscription is already expired");

        subscription.Status    = SubscriptionStatus.Cancelled;
        subscription.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.SubscriptionRepository.PrepareUpdate(subscription);
        await _unitOfWork.CommitTransactionAsync();

        return ServiceResult.Ok("Subscription has been cancelled");
    }

    public async Task<ServiceResult<List<SubscriptionDto>>> GetAllSubscriptionsAsync()
    {
        var subscriptions = await _unitOfWork.SubscriptionRepository.GetAllWithPackageAsync();
        return ServiceResult<List<SubscriptionDto>>.Ok(subscriptions.ToSubscriptionDtoList());
    }

}

