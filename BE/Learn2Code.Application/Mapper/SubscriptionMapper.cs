using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;

namespace Learn2Code.Application.Mapper;

public static class SubscriptionMapper
{
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
    {
        return subscriptions.Select(s => s.ToSubscriptionDto()).ToList();
    }
}
