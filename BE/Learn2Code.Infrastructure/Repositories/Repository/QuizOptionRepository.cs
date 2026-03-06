using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class QuizOptionRepository : GenericRepository<QuizOption>, IQuizOptionRepository
{
    public QuizOptionRepository(Learn2CodeDbContext context) : base(context)
    {
    }

    public async Task<List<QuizOption>> GetOptionsByQuizIdAsync(Guid quizId)
    {
        return await _context.Set<QuizOption>()
            .Where(o => o.QuizId == quizId)
            .ToListAsync();
    }

    public async Task DeleteOptionsByQuizIdAsync(Guid quizId)
    {
        var options = await GetOptionsByQuizIdAsync(quizId);
        _context.Set<QuizOption>().RemoveRange(options);
    }

    public async Task<QuizOption?> GetOptionByIdAsync(Guid quizId, Guid optionId)
    {
        return await _context.Set<QuizOption>()
            .FirstOrDefaultAsync(o => o.QuizId == quizId && o.OptionId == optionId);
    }

    public async Task<int> CountOptionsByQuizIdAsync(Guid quizId)
    {
        return await _context.Set<QuizOption>()
            .CountAsync(o => o.QuizId == quizId);
    }
}
