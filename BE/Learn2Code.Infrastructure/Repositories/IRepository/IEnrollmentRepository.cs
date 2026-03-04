using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface IEnrollmentRepository : IGenericRepository<Enrollment>
{
    Task<List<Enrollment>> GetByStudentIdAsync(Guid studentId);
    Task<Enrollment?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId);
    Task<Enrollment?> GetDetailByIdAsync(Guid enrollmentId);
    Task<Enrollment?> GetDetailByStudentAndCourseAsync(Guid studentId, Guid courseId);
    Task<bool> IsEnrolledAsync(Guid studentId, Guid courseId);
}
