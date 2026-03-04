using Learn2Code.Domain.Entities;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Repositories.Base;
using Learn2Code.Infrastructure.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Infrastructure.Repositories.Repository;

public class ExerciseProgressRepository : GenericRepository<ExerciseProgress>, IExerciseProgressRepository
{
    public ExerciseProgressRepository(Learn2CodeDbContext context) : base(context)
    {
    }

    public async Task<ExerciseProgress?> GetByStudentAndExerciseAsync(Guid studentId, Guid exerciseId)
    {
        return await _context.Set<ExerciseProgress>()
            .FirstOrDefaultAsync(ep => ep.StudentId == studentId && ep.ExerciseId == exerciseId);
    }

    public async Task<List<ExerciseProgress>> GetByStudentAndLessonAsync(Guid studentId, Guid lessonId)
    {
        return await _context.Set<ExerciseProgress>()
            .Include(ep => ep.Exercise)
            .Where(ep => ep.StudentId == studentId && ep.Exercise.LessonId == lessonId)
            .ToListAsync();
    }
}
