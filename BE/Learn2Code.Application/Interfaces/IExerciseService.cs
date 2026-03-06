using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;

namespace Learn2Code.Application.Interfaces;

public interface IExerciseService
{
    Task<ServiceResult<List<ExerciseDto>>> GetExercisesByLessonIdAsync(Guid lessonId);
    Task<ServiceResult<ExerciseDetailDto>> GetExerciseByIdAsync(Guid exerciseId, Guid? userId);
    Task<ServiceResult<ExerciseDto>> CreateExerciseAsync(Guid lessonId, CreateExerciseRequest request);
    Task<ServiceResult<ExerciseDto>> UpdateExerciseAsync(Guid exerciseId, UpdateExerciseRequest request);
    Task<ServiceResult> DeleteExerciseAsync(Guid exerciseId);
}
