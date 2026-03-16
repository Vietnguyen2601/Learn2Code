using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class SectionRepository : GenericRepository<Section>, ISectionRepository
{
    public SectionRepository(Learn2CodeDbContext context) : base(context)
    {
    }

    public async Task<List<Section>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Set<Section>()
            .Where(s => s.CourseId == courseId && s.IsActive)
            .OrderBy(s => s.OrderNumber)
            .ToListAsync();
    }

    public async Task<Section?> GetByIdWithCourseAsync(Guid sectionId)
    {
        return await _context.Set<Section>()
            .Include(s => s.Course)
            .FirstOrDefaultAsync(s => s.SectionId == sectionId);
    }

    public async Task<bool> HasDuplicateOrderInCourseAsync(Guid courseId, int orderNumber, Guid? excludeSectionId = null)
    {
        var query = _context.Set<Section>()
            .Where(s => s.CourseId == courseId && s.OrderNumber == orderNumber);

        if (excludeSectionId.HasValue)
        {
            query = query.Where(s => s.SectionId != excludeSectionId.Value);
        }

        return await query.AnyAsync();
    }
}
