using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class LessonProgressRepository : GenericRepository<LessonProgress>, ILessonProgressRepository
{
    public LessonProgressRepository(Learn2CodeDbContext context) : base(context)
    {
    }

    public async Task<LessonProgress?> GetByStudentAndLessonAsync(Guid studentId, Guid lessonId)
    {
        return await _context.Set<LessonProgress>()
            .FirstOrDefaultAsync(lp => lp.StudentId == studentId && lp.LessonId == lessonId);
    }

    public async Task<List<LessonProgress>> GetByStudentAndCourseAsync(Guid studentId, Guid courseId)
    {
        return await _context.Set<LessonProgress>()
            .Include(lp => lp.Lesson)
                .ThenInclude(l => l.Section)
            .Where(lp => lp.StudentId == studentId && lp.Lesson.Section.CourseId == courseId)
            .ToListAsync();
    }
}
