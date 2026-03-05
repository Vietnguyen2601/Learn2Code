using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;

namespace Learn2Code.Application.Interfaces;

public interface ISubscriptionService
{
    Task<ServiceResult<SubscriptionDto>> GetCurrentSubscriptionAsync(Guid userId);
    Task<ServiceResult<CreateSubscriptionResponse>> CreateSubscriptionAsync(Guid userId, CreateSubscriptionRequest request);
    Task<ServiceResult<CreateSubscriptionResponse>> RenewSubscriptionAsync(Guid userId, Guid subscriptionId);
    Task<ServiceResult> CancelSubscriptionAsync(Guid userId, Guid subscriptionId);
    Task<ServiceResult<List<SubscriptionDto>>> GetAllSubscriptionsAsync();
}
