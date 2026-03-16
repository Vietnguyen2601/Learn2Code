using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Repositories.Base;

namespace Learn2Code.Infrastructure.Repositories.IRepository;

public interface ITestCaseRepository : IGenericRepository<TestCase>
{
    Task<List<TestCase>> GetTestCasesByExerciseIdAsync(Guid exerciseId);
    Task<bool> ExistsInExerciseAsync(Guid exerciseId, Guid testCaseId);
}
