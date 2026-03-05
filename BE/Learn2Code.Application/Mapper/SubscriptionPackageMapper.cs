using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;

namespace Learn2Code.Application.Mapper;

public static class SubscriptionPackageMapper
{
    // ─── Entity factories ────────────────────────────────────────────────────

    public static SubscriptionPackage ToNewPackage(this CreateSubscriptionPackageRequest request)
    {
        var now = DateTime.UtcNow;
        return new SubscriptionPackage
        {
            PackageId       = Guid.NewGuid(),
            Name            = request.Name,
            DurationMonths  = request.DurationMonths,
            Price           = request.Price,
            DiscountPercent = request.DiscountPercent,
            Description     = request.Description,
            IsActive        = true,
            CreatedAt       = now,
            UpdatedAt       = now
        };
    }

    public static void ApplyUpdate(this SubscriptionPackage package, UpdateSubscriptionPackageRequest request)
    {
        if (request.DurationMonths.HasValue)  package.DurationMonths  = request.DurationMonths.Value;
        if (request.Price.HasValue)           package.Price           = request.Price.Value;
        if (request.DiscountPercent.HasValue) package.DiscountPercent = request.DiscountPercent.Value;
        if (request.Description != null)      package.Description     = request.Description;
        if (request.IsActive.HasValue)        package.IsActive        = request.IsActive.Value;
        package.UpdatedAt = DateTime.UtcNow;
    }

    // ─── DTO factories ────────────────────────────────────────────────────

    public static SubscriptionPackageDto ToSubscriptionPackageDto(this SubscriptionPackage package)
    {
        return new SubscriptionPackageDto
        {
            PackageId       = package.PackageId,
            Name            = package.Name,
            DurationMonths  = package.DurationMonths,
            Price           = package.Price,
            DiscountPercent = package.DiscountPercent,
            Description     = package.Description,
            IsActive        = package.IsActive,
            CreatedAt       = package.CreatedAt,
            UpdatedAt       = package.UpdatedAt
        };
    }

    public static List<SubscriptionPackageDto> ToSubscriptionPackageDtoList(this IEnumerable<SubscriptionPackage> packages)
        => packages.Select(p => p.ToSubscriptionPackageDto()).ToList();
}
