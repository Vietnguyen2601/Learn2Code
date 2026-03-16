using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class CourseCategoryRepository : GenericRepository<CourseCategory>, ICourseCategoryRepository
{
    public CourseCategoryRepository(Learn2CodeDbContext context) : base(context)
    {
    }

    public async Task<List<CourseCategory>> GetAllActiveAsync()
    {
        return await _context.Set<CourseCategory>()
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<CourseCategory?> GetByNameAsync(string name)
    {
        return await _context.Set<CourseCategory>()
            .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
    }
}
