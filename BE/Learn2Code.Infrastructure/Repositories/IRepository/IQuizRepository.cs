using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface IQuizRepository : IGenericRepository<Quiz>
{
    Task<List<Quiz>> GetQuizzesByLessonIdAsync(Guid lessonId);
    Task<List<Quiz>> GetQuizzesBySectionIdAsync(Guid sectionId);
    Task<Quiz?> GetQuizWithOptionsAsync(Guid quizId);
    Task<int> GetMaxOrderNumberInLessonAsync(Guid lessonId);
    Task<bool> ExistsInLessonAsync(Guid lessonId, Guid quizId);
}
