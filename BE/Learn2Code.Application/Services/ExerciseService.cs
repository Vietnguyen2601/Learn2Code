using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;

namespace Learn2Code.Application.Services;

public class ExerciseService : IExerciseService
{
    private readonly IUnitOfWork _unitOfWork;

    public ExerciseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<RunCodeResponse>> RunCodeAsync(
        Guid exerciseId, 
        Guid studentId, 
        RunCodeRequest request)
    {
        // Verify exercise exists
        var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(exerciseId);
        
        if (exercise == null)
        {
            return ServiceResult<RunCodeResponse>.NotFound("Exercise not found");
        }

        // Verify exercise type is FreeCode
        if (exercise.ExerciseType != ExerciseType.FreeCode)
        {
            return ServiceResult<RunCodeResponse>.Error(
                "INVALID_EXERCISE_TYPE",
                "This exercise type does not support run code. Use submit instead.",
                400);
        }

        // TODO: Call code execution service (Mock for now)
        var runResult = await ExecuteCodeAsync(request.Code, request.Language, request.Input);

        return ServiceResult<RunCodeResponse>.Ok(runResult);
    }

    public async Task<ServiceResult<SubmitCodeResponse>> SubmitCodeAsync(
        Guid exerciseId, 
        Guid studentId, 
        SubmitCodeRequest request)
    {
        // Get exercise with test cases
        var exercise = await _unitOfWork.ExerciseRepository.GetWithTestCasesAsync(exerciseId);
        
        if (exercise == null)
        {
            return ServiceResult<SubmitCodeResponse>.NotFound("Exercise not found");
        }

        // Verify exercise type is GradedCode
        if (exercise.ExerciseType != ExerciseType.GradedCode)
        {
            return ServiceResult<SubmitCodeResponse>.Error(
                "INVALID_EXERCISE_TYPE",
                "This exercise type does not support code submission.",
                400);
        }

        var testCases = exercise.TestCases.ToList();
        
        if (!testCases.Any())
        {
            return ServiceResult<SubmitCodeResponse>.Error(
                "NO_TEST_CASES",
                "No test cases found for this exercise.",
                500);
        }

        // Run code against all test cases
        var results = new List<TestCaseResultDto>();
        var passedCount = 0;

        foreach (var testCase in testCases)
        {
            // TODO: Call code execution service with test case input
            var runResult = await ExecuteCodeAsync(
                request.Code, 
                request.Language, 
                testCase.ExpectedOutput); // Mock: using expected as input

            var isPassed = runResult.IsSuccess && 
                           runResult.Output?.Trim() == testCase.ExpectedOutput.Trim();

            if (isPassed) passedCount++;

            results.Add(new TestCaseResultDto
            {
                TestCaseId = testCase.TestCaseId,
                IsPassed = isPassed,
                ExpectedOutput = testCase.IsHidden ? null : testCase.ExpectedOutput,
                ActualOutput = testCase.IsHidden ? null : runResult.Output,
                Error = runResult.Error,
                RuntimeMs = runResult.RuntimeMs,
                IsHidden = testCase.IsHidden
            });
        }

        var allPassed = passedCount == testCases.Count;

        var response = new SubmitCodeResponse
        {
            IsPassed = allPassed,
            PassedCount = passedCount,
            TotalCount = testCases.Count,
            Results = results,
            Message = allPassed 
                ? "All test cases passed! Great job!" 
                : $"Passed {passedCount}/{testCases.Count} test cases. Keep trying!"
        };

        // Auto-update progress if passed
        if (allPassed)
        {
            var progressRequest = new UpdateProgressRequest
            {
                IsCompleted = true,
                IsPassed = true,
                LastCode = request.Code
            };

            await UpdateProgressAsync(exerciseId, studentId, progressRequest);
        }

        return ServiceResult<SubmitCodeResponse>.Ok(response);
    }

    public async Task<ServiceResult<ExerciseProgressDto>> UpdateProgressAsync(
        Guid exerciseId, 
        Guid studentId, 
        UpdateProgressRequest request)
    {
        var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(exerciseId);
        
        if (exercise == null)
        {
            return ServiceResult<ExerciseProgressDto>.NotFound("Exercise not found");
        }

        // Get or create exercise progress
        var progress = await _unitOfWork.ExerciseProgressRepository
            .GetByStudentAndExerciseAsync(studentId, exerciseId);

        if (progress == null)
        {
            progress = new Domain.Entities.ExerciseProgress
            {
                ExProgressId = Guid.NewGuid(),
                StudentId = studentId,
                ExerciseId = exerciseId,
                IsCompleted = false,
                IsPassed = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            progress.UpdateFromRequest(request);
            _unitOfWork.ExerciseProgressRepository.PrepareCreate(progress);
        }
        else
        {
            progress.UpdateFromRequest(request);
            _unitOfWork.ExerciseProgressRepository.PrepareUpdate(progress);
        }

        await _unitOfWork.CommitTransactionAsync();

        // Check if all exercises in lesson are completed
        var lessonCompleted = await CheckAndUpdateLessonProgressAsync(
            studentId, 
            exercise.LessonId);

        var message = lessonCompleted 
            ? "Exercise completed! You've finished this lesson!" 
            : "Exercise progress saved!";

        var result = progress.ToExerciseProgressDto(lessonCompleted, message);

        return ServiceResult<ExerciseProgressDto>.Ok(result);
    }

    #region Private Helper Methods

    /// <summary>
    /// Mock code execution service
    /// TODO: Replace with actual code execution service integration
    /// </summary>
    private async Task<RunCodeResponse> ExecuteCodeAsync(
        string code, 
        string language, 
        string? input)
    {
        // Simulate async execution
        await Task.Delay(100);

        // Mock response
        return new RunCodeResponse
        {
            Output = $"Mock output for {language} code",
            RuntimeMs = new Random().Next(50, 200),
            Error = null,
            IsSuccess = true
        };
    }

    /// <summary>
    /// Check if all exercises in lesson are completed and update lesson progress
    /// </summary>
    private async Task<bool> CheckAndUpdateLessonProgressAsync(Guid studentId, Guid lessonId)
    {
        // Get all exercises in lesson
        var exercises = await _unitOfWork.ExerciseRepository.GetByLessonIdAsync(lessonId);
        
        if (!exercises.Any())
        {
            return false;
        }

        // Get all exercise progresses for this lesson
        var exerciseProgresses = await _unitOfWork.ExerciseProgressRepository
            .GetByStudentAndLessonAsync(studentId, lessonId);

        // Check if all exercises are completed
        var allCompleted = exercises.All(e =>
            exerciseProgresses.Any(ep => 
                ep.ExerciseId == e.ExerciseId && 
                ep.IsCompleted));

        if (!allCompleted)
        {
            return false;
        }

        // Update lesson progress to completed
        var lessonProgress = await _unitOfWork.LessonProgressRepository
            .GetByStudentAndLessonAsync(studentId, lessonId);

        if (lessonProgress == null)
        {
            lessonProgress = new Domain.Entities.LessonProgress
            {
                ProgressId = Guid.NewGuid(),
                StudentId = studentId,
                LessonId = lessonId,
                Status = LessonProgressStatus.Completed,
                CompletedAt = DateTime.UtcNow,
                LastAccessedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _unitOfWork.LessonProgressRepository.PrepareCreate(lessonProgress);
        }
        else if (lessonProgress.Status != LessonProgressStatus.Completed)
        {
            lessonProgress.Status = LessonProgressStatus.Completed;
            lessonProgress.CompletedAt = DateTime.UtcNow;
            lessonProgress.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.LessonProgressRepository.PrepareUpdate(lessonProgress);
        }

        await _unitOfWork.CommitTransactionAsync();

        return true;
    }

    #endregion
}
