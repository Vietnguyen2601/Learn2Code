using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface ILessonRepository : IGenericRepository<Lesson>
{
    Task<Lesson?> GetWithExercisesAsync(Guid lessonId);
    Task<List<Lesson>> GetBySectionIdAsync(Guid sectionId);
}
