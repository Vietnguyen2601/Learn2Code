using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;

namespace Learn2Code.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<List<CategoryDto>>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.CourseCategoryRepository.GetAllActiveAsync();
        var categoryDtos = categories.Select(c => c.ToDto()).ToList();

        return ServiceResult<List<CategoryDto>>.Ok(categoryDtos);
    }

    public async Task<ServiceResult<CategoryDto>> GetCategoryByIdAsync(Guid id)
    {
        var category = await _unitOfWork.CourseCategoryRepository.GetByIdAsync(id);
        if (category == null)
            return ServiceResult<CategoryDto>.NotFound("Category not found");

        return ServiceResult<CategoryDto>.Ok(category.ToDto());
    }

    public async Task<ServiceResult<CategoryDto>> CreateCategoryAsync(CreateCategoryRequest request)
    {
        // Validate unique name
        var existing = await _unitOfWork.CourseCategoryRepository.GetByNameAsync(request.Name);
        if (existing != null)
            return ServiceResult<CategoryDto>.Error("CATEGORY_NAME_EXISTS", "Category name already exists");

        var category = request.ToEntity();
        _unitOfWork.CourseCategoryRepository.PrepareCreate(category);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<CategoryDto>.Created(category.ToDto(), "Category created successfully");
    }

    public async Task<ServiceResult<CategoryDto>> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request)
    {
        var category = await _unitOfWork.CourseCategoryRepository.GetByIdAsync(id);
        if (category == null)
            return ServiceResult<CategoryDto>.NotFound("Category not found");

        // Validate unique name if changing
        if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != category.Name)
        {
            var existing = await _unitOfWork.CourseCategoryRepository.GetByNameAsync(request.Name);
            if (existing != null)
                return ServiceResult<CategoryDto>.Error("CATEGORY_NAME_EXISTS", "Category name already exists");
        }

        category.UpdateCategory(request);
        _unitOfWork.CourseCategoryRepository.PrepareUpdate(category);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<CategoryDto>.Ok(category.ToDto(), "Category updated successfully");
    }
}
