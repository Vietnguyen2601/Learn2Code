using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface ILessonRepository : IGenericRepository<Lesson>
{
    Task<List<Lesson>> GetLessonsBySectionIdAsync(Guid sectionId);
    Task<Lesson?> GetLessonWithDetailsAsync(Guid lessonId);
    Task<bool> CanUserAccessLessonAsync(Guid lessonId, Guid? userId);
    Task<int> GetMaxOrderNumberInSectionAsync(Guid sectionId);
    Task<bool> ExistsInSectionAsync(Guid sectionId, Guid lessonId);
}
