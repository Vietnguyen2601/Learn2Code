using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface ILessonProgressRepository : IGenericRepository<LessonProgress>
{
    Task<LessonProgress?> GetByStudentAndLessonAsync(Guid studentId, Guid lessonId);
    Task<List<LessonProgress>> GetByStudentAndCourseAsync(Guid studentId, Guid courseId);
}
