using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface ISectionQuizAttemptRepository : IGenericRepository<SectionQuizAttempt>
{
    Task<List<SectionQuizAttempt>> GetByStudentAndSectionAsync(Guid studentId, Guid sectionId);
    Task<SectionQuizAttempt?> GetWithAnswersAsync(Guid attemptId);
}
