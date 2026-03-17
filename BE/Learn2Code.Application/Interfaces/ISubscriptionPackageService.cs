using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.SubscriptionDTOs.SubscriptionRequests;
using Learn2Code.Application.DTOs.SubscriptionDTOs.SubscriptionResponses;

namespace Learn2Code.Application.Interfaces;

public interface ISubscriptionPackageService
{
    Task<ServiceResult<List<SubscriptionPackageDto>>> GetAllAsync();
    Task<ServiceResult<SubscriptionPackageDto>> GetByIdAsync(Guid id);
    Task<ServiceResult<SubscriptionPackageDto>> CreateAsync(CreateSubscriptionPackageRequest request);
    Task<ServiceResult<SubscriptionPackageDto>> UpdateAsync(Guid id, UpdateSubscriptionPackageRequest request);
    Task<ServiceResult> DisableAsync(Guid id);
}
