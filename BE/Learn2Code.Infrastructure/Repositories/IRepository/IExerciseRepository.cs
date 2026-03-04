using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface IExerciseRepository : IGenericRepository<Exercise>
{
    Task<Exercise?> GetWithTestCasesAsync(Guid exerciseId);
    Task<List<Exercise>> GetByLessonIdAsync(Guid lessonId);
}
