using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface IExerciseProgressRepository : IGenericRepository<ExerciseProgress>
{
    Task<ExerciseProgress?> GetByStudentAndExerciseAsync(Guid studentId, Guid exerciseId);
    Task<List<ExerciseProgress>> GetByStudentAndLessonAsync(Guid studentId, Guid lessonId);
}
