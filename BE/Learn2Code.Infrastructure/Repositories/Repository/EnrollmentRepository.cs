using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class EnrollmentRepository : GenericRepository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(Learn2CodeDbContext context) : base(context)
    {
    }

    public async Task<List<Enrollment>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.Set<Enrollment>()
            .Include(e => e.Course)
            .Where(e => e.StudentId == studentId)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();
    }

    public async Task<Enrollment?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId)
    {
        return await _context.Set<Enrollment>()
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
    }

    public async Task<Enrollment?> GetDetailByIdAsync(Guid enrollmentId)
    {
        return await _context.Set<Enrollment>()
            .Include(e => e.Course)
                .ThenInclude(c => c.Sections.OrderBy(s => s.OrderNumber))
                .ThenInclude(s => s.Lessons.OrderBy(l => l.OrderNumber))
            .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);
    }

    public async Task<Enrollment?> GetDetailByStudentAndCourseAsync(Guid studentId, Guid courseId)
    {
        return await _context.Set<Enrollment>()
            .Include(e => e.Course)
                .ThenInclude(c => c.Sections.OrderBy(s => s.OrderNumber))
                .ThenInclude(s => s.Lessons.OrderBy(l => l.OrderNumber))
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
    }

    public async Task<bool> IsEnrolledAsync(Guid studentId, Guid courseId)
    {
        return await _context.Set<Enrollment>()
            .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
    }
}
