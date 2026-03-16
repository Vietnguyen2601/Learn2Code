using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;

namespace Learn2Code.Application.Interfaces;

public interface ICategoryService
{
    Task<ServiceResult<List<CategoryDto>>> GetAllCategoriesAsync();
    Task<ServiceResult<CategoryDto>> GetCategoryByIdAsync(Guid id);
    Task<ServiceResult<CategoryDto>> CreateCategoryAsync(CreateCategoryRequest request);
    Task<ServiceResult<CategoryDto>> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request);
}
