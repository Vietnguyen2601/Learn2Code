using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;

namespace Learn2Code.Application.Services;

public class SubscriptionPackageService : ISubscriptionPackageService
{
    private readonly IUnitOfWork _unitOfWork;

    public SubscriptionPackageService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<List<SubscriptionPackageDto>>> GetAllAsync()
    {
        var packages = await _unitOfWork.SubscriptionPackageRepository.GetAllActiveOrderedByPriceAsync();
        return ServiceResult<List<SubscriptionPackageDto>>.Ok(packages.ToSubscriptionPackageDtoList());
    }

    public async Task<ServiceResult<SubscriptionPackageDto>> GetByIdAsync(Guid id)
    {
        var package = await _unitOfWork.SubscriptionPackageRepository.GetByIdAsync(id);
        if (package == null)
            return ServiceResult<SubscriptionPackageDto>.NotFound("Subscription package not found");

        return ServiceResult<SubscriptionPackageDto>.Ok(package.ToSubscriptionPackageDto());
    }

    public async Task<ServiceResult<SubscriptionPackageDto>> CreateAsync(CreateSubscriptionPackageRequest request)
    {
        var nameExists = await _unitOfWork.SubscriptionPackageRepository
            .AnyAsync(p => p.Name == request.Name);
        if (nameExists)
            return ServiceResult<SubscriptionPackageDto>.Error("NAME_EXISTS", "A package with this name already exists");

        var package = new SubscriptionPackage
        {
            PackageId      = Guid.NewGuid(),
            Name           = request.Name,
            DurationMonths = request.DurationMonths,
            Price          = request.Price,
            DiscountPercent = request.DiscountPercent,
            Description    = request.Description,
            IsActive       = true,
            CreatedAt      = DateTime.UtcNow,
            UpdatedAt      = DateTime.UtcNow
        };

        _unitOfWork.SubscriptionPackageRepository.PrepareCreate(package);
        await _unitOfWork.CommitTransactionAsync();

        return ServiceResult<SubscriptionPackageDto>.Created(package.ToSubscriptionPackageDto());
    }

    public async Task<ServiceResult<SubscriptionPackageDto>> UpdateAsync(Guid id, UpdateSubscriptionPackageRequest request)
    {
        var package = await _unitOfWork.SubscriptionPackageRepository.GetByIdAsync(id);
        if (package == null)
            return ServiceResult<SubscriptionPackageDto>.NotFound("Subscription package not found");

        // Check name uniqueness if changed
        if (request.Name != null && request.Name != package.Name)
        {
            var nameExists = await _unitOfWork.SubscriptionPackageRepository
                .AnyAsync(p => p.Name == request.Name && p.PackageId != id);
            if (nameExists)
                return ServiceResult<SubscriptionPackageDto>.Error("NAME_EXISTS", "A package with this name already exists");

            package.Name = request.Name;
        }

        if (request.DurationMonths.HasValue) package.DurationMonths = request.DurationMonths.Value;
        if (request.Price.HasValue)          package.Price           = request.Price.Value;
        if (request.DiscountPercent.HasValue) package.DiscountPercent = request.DiscountPercent.Value;
        if (request.Description != null)     package.Description     = request.Description;
        if (request.IsActive.HasValue)        package.IsActive        = request.IsActive.Value;

        package.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.SubscriptionPackageRepository.PrepareUpdate(package);
        await _unitOfWork.CommitTransactionAsync();

        return ServiceResult<SubscriptionPackageDto>.Ok(package.ToSubscriptionPackageDto());
    }

    public async Task<ServiceResult> DisableAsync(Guid id)
    {
        var package = await _unitOfWork.SubscriptionPackageRepository.GetByIdAsync(id);
        if (package == null)
            return ServiceResult.NotFound("Subscription package not found");

        package.IsActive  = false;
        package.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.SubscriptionPackageRepository.PrepareUpdate(package);
        await _unitOfWork.CommitTransactionAsync();

        return ServiceResult.Ok("Subscription package has been disabled");
    }
}
