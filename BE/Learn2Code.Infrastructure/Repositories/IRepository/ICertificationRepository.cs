using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface ICertificationRepository : IGenericRepository<Certification>
{
    /// <summary>
    /// Get all certifications for a student with course details
    /// </summary>
    Task<List<Certification>> GetByStudentIdAsync(Guid studentId);

    /// <summary>
    /// Get certification by certificate code (for public verification)
    /// </summary>
    Task<Certification?> GetByCertificateCodeAsync(string certificateCode);

    /// <summary>
    /// Get all certifications with student and course details (Admin)
    /// </summary>
    Task<List<Certification>> GetAllWithDetailsAsync();

    /// <summary>
    /// Check if student already has certification for a course
    /// </summary>
    Task<bool> ExistsAsync(Guid studentId, Guid courseId);

    /// <summary>
    /// Get certification by student and course
    /// </summary>
    Task<Certification?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId);
}
