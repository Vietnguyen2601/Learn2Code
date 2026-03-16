using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface ICourseCategoryRepository : IGenericRepository<CourseCategory>
{
    Task<List<CourseCategory>> GetAllActiveAsync();
    Task<CourseCategory?> GetByNameAsync(string name);
}
