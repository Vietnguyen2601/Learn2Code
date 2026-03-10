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

    public async Task<List<Enrollment>> GetEnrollmentsByStudentAsync(Guid studentId)
    {
        return await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Course)
            .Where(e => e.StudentId == studentId)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();
    }

    public async Task<Enrollment?> GetEnrollmentByStudentAndCourseAsync(Guid studentId, Guid courseId)
    {
        return await _context.Enrollments
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
    }

    public async Task<Enrollment?> GetEnrollmentWithDetailsAsync(Guid enrollmentId)
    {
        return await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Course)
            .Include(e => e.Student)
            .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);
    }

    public async Task<List<Enrollment>> GetAllWithDetailsAsync()
    {
        return await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Course)
            .Include(e => e.Student)
            .OrderByDescending(e => e.EnrolledAt)
            .ToListAsync();
    }
}
