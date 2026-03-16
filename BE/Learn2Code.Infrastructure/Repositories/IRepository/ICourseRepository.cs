using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface ICourseRepository : IGenericRepository<Course>
{
    Task<List<Course>> GetAllWithRelationsAsync(
        Guid? categoryId = null, 
        CourseDifficulty? difficulty = null, 
        string? search = null);
    
    Task<List<Course>> GetAllWithRelationsByStatusAsync(
        bool isActive,
        Guid? categoryId = null,
        CourseDifficulty? difficulty = null,
        string? search = null);
    
    Task<Course?> GetByIdWithRelationsAsync(Guid id);
}
