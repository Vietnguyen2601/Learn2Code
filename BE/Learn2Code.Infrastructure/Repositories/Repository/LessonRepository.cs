using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
{
    public LessonRepository(Learn2CodeDbContext context) : base(context)
    {
    }

    public async Task<Lesson?> GetWithExercisesAsync(Guid lessonId)
    {
        return await _context.Set<Lesson>()
            .Include(l => l.Exercises.OrderBy(e => e.OrderNumber))
                .ThenInclude(e => e.ExerciseMedias.OrderBy(m => m.OrderNumber))
            .FirstOrDefaultAsync(l => l.LessonId == lessonId);
    }

    public async Task<List<Lesson>> GetBySectionIdAsync(Guid sectionId)
    {
        return await _context.Set<Lesson>()
            .Where(l => l.SectionId == sectionId)
            .OrderBy(l => l.OrderNumber)
            .ToListAsync();
    }
}
