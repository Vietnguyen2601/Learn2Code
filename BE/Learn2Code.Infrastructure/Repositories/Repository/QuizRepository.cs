using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class QuizRepository : GenericRepository<Quiz>, IQuizRepository
{
    public QuizRepository(Learn2CodeDbContext context) : base(context)
    {
    }

    public async Task<List<Quiz>> GetQuizzesByLessonIdAsync(Guid lessonId)
    {
        return await _context.Set<Quiz>()
            .Include(q => q.Options)
            .Where(q => q.LessonId == lessonId)
            .OrderBy(q => q.OrderNumber)
            .ToListAsync();
    }

    public async Task<List<Quiz>> GetQuizzesBySectionIdAsync(Guid sectionId)
    {
        return await _context.Set<Quiz>()
            .Include(q => q.Options)
            .Include(q => q.Lesson)
            .Where(q => q.Lesson.SectionId == sectionId)
            .OrderBy(q => q.Lesson.OrderNumber)
                .ThenBy(q => q.OrderNumber)
            .ToListAsync();
    }

    public async Task<Quiz?> GetQuizWithOptionsAsync(Guid quizId)
    {
        return await _context.Set<Quiz>()
            .Include(q => q.Options)
            .Include(q => q.Lesson)
                .ThenInclude(l => l.Section)
                .ThenInclude(s => s.Course)
            .FirstOrDefaultAsync(q => q.QuizId == quizId);
    }

    public async Task<int> GetMaxOrderNumberInLessonAsync(Guid lessonId)
    {
        var maxOrder = await _context.Set<Quiz>()
            .Where(q => q.LessonId == lessonId)
            .MaxAsync(q => (int?)q.OrderNumber);

        return maxOrder ?? 0;
    }

    public async Task<bool> ExistsInLessonAsync(Guid lessonId, Guid quizId)
    {
        return await _context.Set<Quiz>()
            .AnyAsync(q => q.LessonId == lessonId && q.QuizId == quizId);
    }
}
