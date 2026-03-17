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
            .AsNoTracking()
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

    public async Task ShiftOrderNumbersUpAsync(Guid courseId, int startingOrder)
    {
        var tempSql = "UPDATE sections SET order_number = order_number + 1000000, updated_at = {0} WHERE course_id = {1} AND order_number >= {2}";
        await _context.Database.ExecuteSqlRawAsync(tempSql, DateTime.UtcNow, courseId, startingOrder);

        var finalSql = "UPDATE sections SET order_number = order_number - 999999, updated_at = {0} WHERE course_id = {1} AND order_number >= {2} + 1000000";
        await _context.Database.ExecuteSqlRawAsync(finalSql, DateTime.UtcNow, courseId, startingOrder);
    }

    public async Task ShiftOrderRangeAsync(Guid courseId, int startOrderInclusive, int endOrderInclusive, int delta)
    {
        if (delta == 0)
            return;

        var tempSql = "UPDATE sections SET order_number = order_number + 1000000, updated_at = {0} WHERE course_id = {1} AND order_number >= {2} AND order_number <= {3}";
        await _context.Database.ExecuteSqlRawAsync(tempSql, DateTime.UtcNow, courseId, startOrderInclusive, endOrderInclusive);

        var finalSql = "UPDATE sections SET order_number = order_number + {0} - 1000000, updated_at = {1} WHERE course_id = {2} AND order_number >= {3} + 1000000 AND order_number <= {4} + 1000000";
        await _context.Database.ExecuteSqlRawAsync(finalSql, delta, DateTime.UtcNow, courseId, startOrderInclusive, endOrderInclusive);
    }

    public async Task MoveSectionToOrderAsync(Guid sectionId, int orderNumber)
    {
        var sql = "UPDATE sections SET order_number = {0}, updated_at = {1} WHERE section_id = {2}";
        await _context.Database.ExecuteSqlRawAsync(sql, orderNumber, DateTime.UtcNow, sectionId);
    }
}
