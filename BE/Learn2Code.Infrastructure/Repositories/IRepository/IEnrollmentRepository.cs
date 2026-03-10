using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface IEnrollmentRepository : IGenericRepository<Enrollment>
{
    Task<List<Enrollment>> GetEnrollmentsByStudentAsync(Guid studentId);
    Task<Enrollment?> GetEnrollmentByStudentAndCourseAsync(Guid studentId, Guid courseId);
    Task<Enrollment?> GetEnrollmentWithDetailsAsync(Guid enrollmentId);
    Task<List<Enrollment>> GetAllWithDetailsAsync();
}
