using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class CertificationRepository : GenericRepository<Certification>, ICertificationRepository
{
    public CertificationRepository(Learn2CodeDbContext context) : base(context)
    {
    }

    public async Task<List<Certification>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.Certifications
            .AsNoTracking()
            .Include(c => c.Course)
            .Include(c => c.Student)
            .Where(c => c.StudentId == studentId)
            .OrderByDescending(c => c.IssuedAt)
            .ToListAsync();
    }

    public async Task<Certification?> GetByCertificateCodeAsync(string certificateCode)
    {
        return await _context.Certifications
            .AsNoTracking()
            .Include(c => c.Course)
            .Include(c => c.Student)
            .FirstOrDefaultAsync(c => c.CertificateCode == certificateCode);
    }

    public async Task<List<Certification>> GetAllWithDetailsAsync()
    {
        return await _context.Certifications
            .AsNoTracking()
            .Include(c => c.Course)
            .Include(c => c.Student)
            .OrderByDescending(c => c.IssuedAt)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid studentId, Guid courseId)
    {
        return await _context.Certifications
            .AnyAsync(c => c.StudentId == studentId && c.CourseId == courseId);
    }

    public async Task<Certification?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId)
    {
        return await _context.Certifications
            .AsNoTracking()
            .Include(c => c.Course)
            .Include(c => c.Student)
            .FirstOrDefaultAsync(c => c.StudentId == studentId && c.CourseId == courseId);
    }
}
