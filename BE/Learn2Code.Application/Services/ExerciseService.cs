using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;

namespace Learn2Code.Application.Services;

public class ExerciseService : IExerciseService
{
    private readonly IUnitOfWork _unitOfWork;

    public ExerciseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<List<ExerciseDto>>> GetExercisesByLessonIdAsync(Guid lessonId)
    {
        // Ki?m tra lesson có t?n t?i không
        var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
            return ServiceResult<List<ExerciseDto>>.NotFound("Lesson not found");

        var exercises = await _unitOfWork.ExerciseRepository.GetExercisesByLessonIdAsync(lessonId);
        var exerciseDtos = exercises.Select(e => e.ToDto()).ToList();

        return ServiceResult<List<ExerciseDto>>.Ok(exerciseDtos);
    }

    public async Task<ServiceResult<ExerciseDetailDto>> GetExerciseByIdAsync(Guid exerciseId, Guid? userId)
    {
        var exercise = await _unitOfWork.ExerciseRepository.GetExerciseWithDetailsAsync(exerciseId);
        if (exercise == null)
            return ServiceResult<ExerciseDetailDto>.NotFound("Exercise not found");

        // Ki?m tra quy?n truy c?p
        var canAccess = await _unitOfWork.ExerciseRepository.CanUserAccessExerciseAsync(exerciseId, userId);
        if (!canAccess)
            return ServiceResult<ExerciseDetailDto>.Error("ACCESS_DENIED", "You don't have permission to access this exercise", 403);

        return ServiceResult<ExerciseDetailDto>.Ok(exercise.ToDetailDto());
    }

    public async Task<ServiceResult<ExerciseDto>> CreateExerciseAsync(Guid lessonId, CreateExerciseRequest request)
    {
        // Ki?m tra lesson có t?n t?i không
        var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
            return ServiceResult<ExerciseDto>.NotFound("Lesson not found");

        // Validate ExerciseType
        if (!Enum.TryParse<Domain.Enums.ExerciseType>(request.ExerciseType, true, out _))
            return ServiceResult<ExerciseDto>.Error("INVALID_EXERCISE_TYPE", "Exercise type must be one of: Reading, FreeCode, GradedCode");

        // L?y order number ti?p theo
        var maxOrder = await _unitOfWork.ExerciseRepository.GetMaxOrderNumberInLessonAsync(lessonId);
        var newOrderNumber = maxOrder + 1;

        var exercise = request.ToEntity(lessonId, newOrderNumber);
        _unitOfWork.ExerciseRepository.PrepareCreate(exercise);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<ExerciseDto>.Created(exercise.ToDto(), "Exercise created successfully");
    }

    public async Task<ServiceResult<ExerciseDto>> UpdateExerciseAsync(Guid exerciseId, UpdateExerciseRequest request)
    {
        var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(exerciseId);
        if (exercise == null)
            return ServiceResult<ExerciseDto>.NotFound("Exercise not found");

        // Validate ExerciseType n?u có update
        if (!string.IsNullOrWhiteSpace(request.ExerciseType))
        {
            if (!Enum.TryParse<Domain.Enums.ExerciseType>(request.ExerciseType, true, out _))
                return ServiceResult<ExerciseDto>.Error("INVALID_EXERCISE_TYPE", "Exercise type must be one of: Reading, FreeCode, GradedCode");
        }

        exercise.UpdateExercise(request);
        _unitOfWork.ExerciseRepository.PrepareUpdate(exercise);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<ExerciseDto>.Ok(exercise.ToDto(), "Exercise updated successfully");
    }

    public async Task<ServiceResult> DeleteExerciseAsync(Guid exerciseId)
    {
        var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(exerciseId);
        if (exercise == null)
            return ServiceResult.NotFound("Exercise not found");

        _unitOfWork.ExerciseRepository.PrepareRemove(exercise);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Ok("Exercise deleted successfully");
    }
}
