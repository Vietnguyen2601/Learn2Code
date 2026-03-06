using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class TestCaseRepository : GenericRepository<TestCase>, ITestCaseRepository
{
    public TestCaseRepository(Learn2CodeDbContext context) : base(context)
    {
    }

    public async Task<List<TestCase>> GetTestCasesByExerciseIdAsync(Guid exerciseId)
    {
        return await _context.Set<TestCase>()
            .Where(tc => tc.ExerciseId == exerciseId)
            .OrderBy(tc => tc.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> ExistsInExerciseAsync(Guid exerciseId, Guid testCaseId)
    {
        return await _context.Set<TestCase>()
            .AnyAsync(tc => tc.ExerciseId == exerciseId && tc.TestCaseId == testCaseId);
    }
}
