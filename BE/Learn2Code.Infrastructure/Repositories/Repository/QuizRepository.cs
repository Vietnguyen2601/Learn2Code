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

    public async Task<Quiz?> GetWithOptionsAsync(Guid quizId)
    {
        return await _context.Set<Quiz>()
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.QuizId == quizId);
    }

    public async Task<List<Quiz>> GetBySectionIdAsync(Guid sectionId)
    {
        return await _context.Set<Quiz>()
            .Include(q => q.Options)
            .Include(q => q.Lesson)
            .Where(q => q.Lesson.SectionId == sectionId)
            .OrderBy(q => q.Lesson.OrderNumber)
            .ThenBy(q => q.OrderNumber)
            .ToListAsync();
    }
}
