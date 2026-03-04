using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class SectionQuizAttemptRepository : GenericRepository<SectionQuizAttempt>, ISectionQuizAttemptRepository
{
    public SectionQuizAttemptRepository(Learn2CodeDbContext context) : base(context)
    {
    }

    public async Task<List<SectionQuizAttempt>> GetByStudentAndSectionAsync(Guid studentId, Guid sectionId)
    {
        return await _context.Set<SectionQuizAttempt>()
            .Include(a => a.Section)
            .Include(a => a.Answers)
            .Where(a => a.StudentId == studentId && a.SectionId == sectionId)
            .OrderByDescending(a => a.AttemptedAt)
            .ToListAsync();
    }

    public async Task<SectionQuizAttempt?> GetWithAnswersAsync(Guid attemptId)
    {
        return await _context.Set<SectionQuizAttempt>()
            .Include(a => a.Section)
            .Include(a => a.Answers)
                .ThenInclude(ans => ans.Quiz)
            .Include(a => a.Answers)
                .ThenInclude(ans => ans.Option)
            .FirstOrDefaultAsync(a => a.AttemptId == attemptId);
    }
}
