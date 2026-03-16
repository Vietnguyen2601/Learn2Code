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

    public async Task<List<Exercise>> GetExercisesByLessonIdAsync(Guid lessonId)
    {
        return await _context.Set<Exercise>()
            .Where(e => e.LessonId == lessonId)
            .OrderBy(e => e.OrderNumber)
            .ToListAsync();
    }

    public async Task<Exercise?> GetExerciseWithDetailsAsync(Guid exerciseId)
    {
        return await _context.Set<Exercise>()
            .Include(e => e.Lesson)
                .ThenInclude(l => l.Section)
                .ThenInclude(s => s.Course)
            .Include(e => e.TestCases)
            .Include(e => e.ExerciseMedias.OrderBy(m => m.OrderNumber))
            .FirstOrDefaultAsync(e => e.ExerciseId == exerciseId);
    }

    public async Task<bool> CanUserAccessExerciseAsync(Guid exerciseId, Guid? userId)
    {
        var exercise = await _context.Set<Exercise>()
            .Include(e => e.Lesson)
                .ThenInclude(l => l.Section)
                .ThenInclude(s => s.Course)
            .FirstOrDefaultAsync(e => e.ExerciseId == exerciseId);

        if (exercise == null)
            return false;

        // If lesson is free preview, anyone can access
        if (exercise.Lesson.IsFreePreview)
            return true;

        // If user is not authenticated, can't access non-free exercises
        if (userId == null)
            return false;

        // Check if user is enrolled in the course
        var isEnrolled = await _context.Set<Enrollment>()
            .AnyAsync(en => en.CourseId == exercise.Lesson.Section.CourseId 
                         && en.StudentId == userId.Value 
                         && en.Status != Domain.Enums.EnrollmentStatus.Completed);

        return isEnrolled;
    }

    public async Task<int> GetMaxOrderNumberInLessonAsync(Guid lessonId)
    {
        var maxOrder = await _context.Set<Exercise>()
            .Where(e => e.LessonId == lessonId)
            .MaxAsync(e => (int?)e.OrderNumber);

        return maxOrder ?? 0;
    }

    public async Task<bool> ExistsInLessonAsync(Guid lessonId, Guid exerciseId)
    {
        return await _context.Set<Exercise>()
            .AnyAsync(e => e.LessonId == lessonId && e.ExerciseId == exerciseId);
    }
}
