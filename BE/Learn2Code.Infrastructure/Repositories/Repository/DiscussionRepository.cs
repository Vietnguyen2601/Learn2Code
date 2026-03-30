using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class DiscussionRepository : GenericRepository<Discussion>, IDiscussionRepository
{
    public DiscussionRepository(Learn2CodeDbContext context) : base(context)
    {
    }

    public async Task<List<Discussion>> GetDiscussionsByLessonIdAsync(Guid lessonId)
    {
        return await _context.Set<Discussion>()
            .Where(d => d.LessonId == lessonId)
            .Include(d => d.Creator)
            .Include(d => d.Comments)
            .AsNoTracking()
            .OrderByDescending(d => d.IsPinned)
            .ThenByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<Discussion?> GetDiscussionWithDetailsAsync(Guid discussionId)
    {
        return await _context.Set<Discussion>()
            .Include(d => d.Creator)
            .Include(d => d.Lesson)
            .Include(d => d.Comments)
                .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(d => d.DiscussionId == discussionId);
    }

    public async Task<List<Discussion>> GetPinnedDiscussionsByLessonIdAsync(Guid lessonId)
    {
        return await _context.Set<Discussion>()
            .Where(d => d.LessonId == lessonId && d.IsPinned)
            .Include(d => d.Creator)
            .Include(d => d.Comments)
            .AsNoTracking()
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }
}
