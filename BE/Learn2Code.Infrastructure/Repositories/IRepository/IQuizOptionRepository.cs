using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface IQuizOptionRepository : IGenericRepository<QuizOption>
{
    Task<List<QuizOption>> GetOptionsByQuizIdAsync(Guid quizId);
    Task DeleteOptionsByQuizIdAsync(Guid quizId);
    Task<QuizOption?> GetOptionByIdAsync(Guid quizId, Guid optionId);
    Task<int> CountOptionsByQuizIdAsync(Guid quizId);
}
