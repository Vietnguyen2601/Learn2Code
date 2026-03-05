using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;

namespace Learn2Code.Application.Mapper;

public static class SubscriptionPackageMapper
{
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
    {
        return packages.Select(p => p.ToSubscriptionPackageDto()).ToList();
    }
}
