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

    public async Task<List<Lesson>> GetLessonsBySectionIdAsync(Guid sectionId)
    {
        return await _context.Set<Lesson>()
            .Where(l => l.SectionId == sectionId)
            .OrderBy(l => l.OrderNumber)
            .ToListAsync();
    }

    public async Task<Lesson?> GetLessonWithDetailsAsync(Guid lessonId)
    {
        return await _context.Set<Lesson>()
            .Include(l => l.Section)
                .ThenInclude(s => s.Course)
            .Include(l => l.Exercises)
            .Include(l => l.Quizzes)
            .FirstOrDefaultAsync(l => l.LessonId == lessonId);
    }

    public async Task<bool> CanUserAccessLessonAsync(Guid lessonId, Guid? userId)
    {
        var lesson = await _context.Set<Lesson>()
            .Include(l => l.Section)
                .ThenInclude(s => s.Course)
            .FirstOrDefaultAsync(l => l.LessonId == lessonId);

        if (lesson == null)
            return false;

        // If lesson is free preview, anyone can access
        if (lesson.IsFreePreview)
            return true;

        // If user is not authenticated, can't access non-free lessons
        if (userId == null)
            return false;

        // Check if user is enrolled in the course
        var isEnrolled = await _context.Set<Enrollment>()
            .AnyAsync(e => e.CourseId == lesson.Section.CourseId 
                        && e.StudentId == userId.Value 
                        && e.Status != Domain.Enums.EnrollmentStatus.Completed);

        return isEnrolled;
    }

    public async Task<int> GetMaxOrderNumberInSectionAsync(Guid sectionId)
    {
        var maxOrder = await _context.Set<Lesson>()
            .Where(l => l.SectionId == sectionId)
            .MaxAsync(l => (int?)l.OrderNumber);

        return maxOrder ?? 0;
    }

    public async Task<bool> ExistsInSectionAsync(Guid sectionId, Guid lessonId)
    {
        return await _context.Set<Lesson>()
            .AnyAsync(l => l.SectionId == sectionId && l.LessonId == lessonId);
    }
}
