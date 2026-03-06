using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface IExerciseRepository : IGenericRepository<Exercise>
{
    Task<List<Exercise>> GetExercisesByLessonIdAsync(Guid lessonId);
    Task<Exercise?> GetExerciseWithDetailsAsync(Guid exerciseId);
    Task<bool> CanUserAccessExerciseAsync(Guid exerciseId, Guid? userId);
    Task<int> GetMaxOrderNumberInLessonAsync(Guid lessonId);
    Task<bool> ExistsInLessonAsync(Guid lessonId, Guid exerciseId);
}
