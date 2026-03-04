using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface IQuizRepository : IGenericRepository<Quiz>
{
    Task<Quiz?> GetWithOptionsAsync(Guid quizId);
    Task<List<Quiz>> GetBySectionIdAsync(Guid sectionId);
}
