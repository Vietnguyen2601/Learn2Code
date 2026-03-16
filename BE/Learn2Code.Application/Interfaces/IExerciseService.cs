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
    Task<ServiceResult<ExerciseProgressDto>> RunCodeAsync(Guid exerciseId, Guid studentId, RunCodeRequest request);
    Task<ServiceResult<ExerciseProgressDto>> SubmitCodeAsync(Guid exerciseId, Guid studentId, SubmitCodeRequest request);
    Task<ServiceResult<ExerciseProgressDto>> UpdateExerciseProgressAsync(Guid exerciseId, Guid studentId, UpdateExerciseProgressRequest request);
    Task<ServiceResult<ExerciseProgressDto>> GetExerciseProgressAsync(Guid exerciseId, Guid studentId);
}
