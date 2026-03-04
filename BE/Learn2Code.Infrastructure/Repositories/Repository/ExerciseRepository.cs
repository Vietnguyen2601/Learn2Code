using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class ExerciseRepository : GenericRepository<Exercise>, IExerciseRepository
{
    public ExerciseRepository(Learn2CodeDbContext context) : base(context)
    {
    }

    public async Task<Exercise?> GetWithTestCasesAsync(Guid exerciseId)
    {
        return await _context.Set<Exercise>()
            .Include(e => e.TestCases)
            .Include(e => e.Lesson)
            .FirstOrDefaultAsync(e => e.ExerciseId == exerciseId);
    }

    public async Task<List<Exercise>> GetByLessonIdAsync(Guid lessonId)
    {
        return await _context.Set<Exercise>()
            .Where(e => e.LessonId == lessonId)
            .OrderBy(e => e.OrderNumber)
            .ToListAsync();
    }
}
