using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.DTOs.ExerciseDTOs.ExerciseRequests;
using Learn2Code.Application.DTOs.ExerciseDTOs.ExerciseResponses;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.DTOs;
using Learn2Code.Infrastructure.Options;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Learn2Code.Infrastructure.Services;
using Microsoft.Extensions.Options;

namespace Learn2Code.Application.Services;

public class ExerciseService : IExerciseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPistonService _pistonService;
    private readonly PistonOptions _pistonOptions;
    private readonly IGamificationService _gamificationService;
    private readonly IDailyStreakService _streakService;

    public ExerciseService(IUnitOfWork unitOfWork, IPistonService pistonService,
        IOptions<PistonOptions> pistonOptions, IGamificationService gamificationService,
        IDailyStreakService streakService)
    {
        _unitOfWork = unitOfWork;
        _pistonService = pistonService;
        _pistonOptions = pistonOptions.Value;
        _gamificationService = gamificationService;
        _streakService = streakService;
    }

    public async Task<ServiceResult<List<ExerciseDto>>> GetExercisesByLessonIdAsync(Guid lessonId)
    {
        // Ki?m tra lesson c� t?n t?i kh�ng
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
        // Ki?m tra lesson c� t?n t?i kh�ng
        var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
            return ServiceResult<ExerciseDto>.NotFound("Lesson not found");

        // Validate ExerciseType
        if (!Enum.TryParse<Domain.Enums.ExerciseType>(request.ExerciseType, true, out var exerciseType))
            return ServiceResult<ExerciseDto>.Error("INVALID_EXERCISE_TYPE", "Exercise type must be one of: Reading, FreeCode, GradedCode");

        var sanitizedRequest = SanitizeCreateRequest(request, exerciseType);

        var exercisesInLesson = await _unitOfWork.ExerciseRepository.GetExercisesByLessonIdAsync(lessonId);
        var maxPosition = exercisesInLesson.Count + 1;
        var desiredOrder = sanitizedRequest.OrderNumber ?? maxPosition;
        desiredOrder = Math.Max(1, Math.Min(desiredOrder, maxPosition));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            await _unitOfWork.ExerciseRepository.ShiftOrderNumbersUpAsync(lessonId, desiredOrder);

            var exercise = sanitizedRequest.ToEntity(lessonId, desiredOrder);
            _unitOfWork.ExerciseRepository.PrepareCreate(exercise);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<ExerciseDto>.Created(exercise.ToDto(), "Exercise created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return ServiceResult<ExerciseDto>.Error("CREATE_EXERCISE_FAILED", $"Failed to create exercise: {ex.Message}", 500);
        }
    }

    public async Task<ServiceResult<ExerciseDto>> UpdateExerciseAsync(Guid exerciseId, UpdateExerciseRequest request)
    {
        var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(exerciseId);
        if (exercise == null)
            return ServiceResult<ExerciseDto>.NotFound("Exercise not found");

        // Validate ExerciseType n?u c� update
        if (!string.IsNullOrWhiteSpace(request.ExerciseType))
        {
            if (!Enum.TryParse<Domain.Enums.ExerciseType>(request.ExerciseType, true, out _))
                return ServiceResult<ExerciseDto>.Error("INVALID_EXERCISE_TYPE", "Exercise type must be one of: Reading, FreeCode, GradedCode");
        }

        var targetTypeString = request.ExerciseType ?? exercise.ExerciseType.ToString();
        var targetType = Enum.Parse<Domain.Enums.ExerciseType>(targetTypeString, true);
        var sanitizedRequest = SanitizeUpdateRequest(request, targetType);
        var clearCodeFields = targetType == Domain.Enums.ExerciseType.Reading;

        var exercisesInLesson = await _unitOfWork.ExerciseRepository.GetExercisesByLessonIdAsync(exercise.LessonId);
        var totalExercises = exercisesInLesson.Count;
        var currentOrder = exercise.OrderNumber;

        var desiredOrder = sanitizedRequest.OrderNumber ?? currentOrder;
        desiredOrder = Math.Max(1, Math.Min(desiredOrder, totalExercises));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (desiredOrder != currentOrder)
            {
                const int tempOrder = 2000000000;
                await _unitOfWork.ExerciseRepository.MoveExerciseToOrderAsync(exerciseId, tempOrder);

                if (desiredOrder < currentOrder)
                {
                    await _unitOfWork.ExerciseRepository.ShiftOrderRangeAsync(exercise.LessonId, desiredOrder, currentOrder - 1, +1);
                }
                else
                {
                    await _unitOfWork.ExerciseRepository.ShiftOrderRangeAsync(exercise.LessonId, currentOrder + 1, desiredOrder, -1);
                }
            }

            exercise.UpdateExercise(sanitizedRequest, desiredOrder, clearCodeFields);
            _unitOfWork.ExerciseRepository.PrepareUpdate(exercise);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return ServiceResult<ExerciseDto>.Ok(exercise.ToDto(), "Exercise updated successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return ServiceResult<ExerciseDto>.Error("UPDATE_EXERCISE_FAILED", $"Failed to update exercise: {ex.Message}", 500);
        }
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

    // ── Progress / Run / Submit ──────────────────────────────────────────────

    public async Task<ServiceResult<ExerciseProgressDto>> RunCodeAsync(Guid exerciseId, Guid studentId, RunCodeRequest request)
    {
        var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(exerciseId);
        if (exercise == null)
            return ServiceResult<ExerciseProgressDto>.NotFound("Exercise not found");

        if (exercise.ExerciseType == ExerciseType.Reading)
            return ServiceResult<ExerciseProgressDto>.Error("INVALID_EXERCISE_TYPE", "Reading exercises do not support code execution");

        var canAccess = await _unitOfWork.ExerciseRepository.CanUserAccessExerciseAsync(exerciseId, studentId);
        if (!canAccess)
            return ServiceResult<ExerciseProgressDto>.Error("ACCESS_DENIED", "You don't have permission to access this exercise", 403);

        var language = ResolveLanguage(request.Language, exercise.Language);
        if (string.IsNullOrWhiteSpace(language))
            return ServiceResult<ExerciseProgressDto>.BadRequest("Language is required. Provide request.language or configure exercise.language");

        var execution = await ExecuteCodeAsync(language, request.Code);
        if (execution.Response == null)
            return ServiceResult<ExerciseProgressDto>.Error("CODE_ENGINE_UNAVAILABLE", "Unable to run code at the moment", 503);

        var progress = await UpsertProgressAsync(studentId, exerciseId, p =>
        {
            p.LastCode = request.Code;
        });

        var response = progress.ToProgressDto();
        ApplyExecutionResult(response, execution.Response, language, execution.RuntimeMs);

        return ServiceResult<ExerciseProgressDto>.Ok(response, "Code executed successfully");
    }

    public async Task<ServiceResult<ExerciseProgressDto>> SubmitCodeAsync(Guid exerciseId, Guid studentId, SubmitCodeRequest request)
    {
        var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(exerciseId);
        if (exercise == null)
            return ServiceResult<ExerciseProgressDto>.NotFound("Exercise not found");

        if (exercise.ExerciseType == ExerciseType.Reading)
            return ServiceResult<ExerciseProgressDto>.Error("INVALID_EXERCISE_TYPE", "Reading exercises do not support code submission");

        var canAccess = await _unitOfWork.ExerciseRepository.CanUserAccessExerciseAsync(exerciseId, studentId);
        if (!canAccess)
            return ServiceResult<ExerciseProgressDto>.Error("ACCESS_DENIED", "You don't have permission to access this exercise", 403);

        var language = ResolveLanguage(request.Language, exercise.Language);
        if (string.IsNullOrWhiteSpace(language))
            return ServiceResult<ExerciseProgressDto>.BadRequest("Language is required. Provide request.language or configure exercise.language");

        var execution = await ExecuteCodeAsync(language, request.Code);
        if (execution.Response == null)
            return ServiceResult<ExerciseProgressDto>.Error("CODE_ENGINE_UNAVAILABLE", "Unable to submit code at the moment", 503);

        var runResult = execution.Response.Run;
        var runSucceeded = runResult?.Code == 0;
        var now = DateTime.UtcNow;
        var testCaseResults = new List<ExerciseTestCaseResultDto>();
        var finalPassed = runSucceeded;

        if (exercise.ExerciseType == ExerciseType.GradedCode)
        {
            var testCases = await _unitOfWork.TestCaseRepository.GetTestCasesByExerciseIdAsync(exerciseId);
            if (testCases.Count == 0)
                return ServiceResult<ExerciseProgressDto>.Error("TEST_CASES_NOT_FOUND", "No test cases found for this graded exercise", 422);

            int accumulatedRuntimeMs = 0;
            (PistonExecuteResponse? Response, int RuntimeMs) lastExecution = (null, 0);

            foreach (var testCase in testCases)
            {
                var testExecution = await ExecuteCodeAsync(language, request.Code, testCase.TextInput);
                if (testExecution.Response == null)
                    return ServiceResult<ExerciseProgressDto>.Error("CODE_ENGINE_UNAVAILABLE", "Unable to submit code at the moment", 503);

                accumulatedRuntimeMs += testExecution.RuntimeMs;
                lastExecution = testExecution;

                var testRunResult = testExecution.Response.Run;
                var testRunSucceeded = testRunResult?.Code == 0;
                var actualOutput = testRunResult?.Stdout ?? testRunResult?.Output ?? string.Empty;
                var normalizedActualOutput = NormalizeOutput(actualOutput);
                var normalizedExpectedOutput = NormalizeOutput(testCase.ExpectedOutput);
                var isPassed = testRunSucceeded && normalizedExpectedOutput == normalizedActualOutput;

                testCaseResults.Add(new ExerciseTestCaseResultDto
                {
                    TestCaseId = testCase.TestCaseId,
                    IsPassed = isPassed,
                    ActualOutput = testCase.IsHidden ? null : actualOutput
                });
            }

            finalPassed = testCaseResults.All(x => x.IsPassed);

            if (lastExecution.Response != null)
            {
                execution = (lastExecution.Response, Math.Max(1, accumulatedRuntimeMs));
            }
        }

        var progress = await UpsertProgressAsync(studentId, exerciseId, p =>
        {
            p.LastCode = request.Code;
            p.IsPassed = finalPassed;
            p.IsCompleted = finalPassed;
            p.CompletedAt = finalPassed ? (p.CompletedAt ?? now) : null;
        });

        var response = progress.ToProgressDto();
        response.TestCaseResults = testCaseResults;
        ApplyExecutionResult(response, execution.Response, language, execution.RuntimeMs);

        if (finalPassed)
        {
            await _gamificationService.ProcessEventAsync(studentId, XPEventType.ExercisePassed);
            // Trigger daily streak check-in (fire-and-forget)
            _ = _streakService.CheckInAsync(studentId);
        }

        var message = finalPassed ? "Submitted successfully" : "Submission failed. Please review your code and try again";
        return ServiceResult<ExerciseProgressDto>.Ok(response, message);
    }

    public async Task<ServiceResult<ExerciseProgressDto>> UpdateExerciseProgressAsync(Guid exerciseId, Guid studentId, UpdateExerciseProgressRequest request)
    {
        var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(exerciseId);
        if (exercise == null)
            return ServiceResult<ExerciseProgressDto>.NotFound("Exercise not found");

        var canAccess = await _unitOfWork.ExerciseRepository.CanUserAccessExerciseAsync(exerciseId, studentId);
        if (!canAccess)
            return ServiceResult<ExerciseProgressDto>.Error("ACCESS_DENIED", "You don't have permission to access this exercise", 403);

        var now = DateTime.UtcNow;
        var progress = await UpsertProgressAsync(studentId, exerciseId, p =>
        {
            p.IsCompleted = request.IsCompleted;
            if (request.IsCompleted)
            {
                p.IsPassed = true;
                p.CompletedAt ??= now;
            }
        });

        return ServiceResult<ExerciseProgressDto>.Ok(progress.ToProgressDto(), "Progress updated successfully");
    }

    public async Task<ServiceResult<ExerciseProgressDto>> GetExerciseProgressAsync(Guid exerciseId, Guid studentId)
    {
        var progress = await _unitOfWork.Repository<ExerciseProgress>()
            .GetAsync(p => p.StudentId == studentId && p.ExerciseId == exerciseId);

        if (progress == null)
            return ServiceResult<ExerciseProgressDto>.NotFound("No progress found for this exercise");

        return ServiceResult<ExerciseProgressDto>.Ok(progress.ToProgressDto());
    }

    private async Task<ExerciseProgress> UpsertProgressAsync(Guid studentId, Guid exerciseId, Action<ExerciseProgress> applyChanges)
    {
        var existing = await _unitOfWork.Repository<ExerciseProgress>()
            .GetAsync(p => p.StudentId == studentId && p.ExerciseId == exerciseId);

        if (existing == null)
        {
            existing = new ExerciseProgress
            {
                ExProgressId = Guid.NewGuid(),
                StudentId = studentId,
                ExerciseId = exerciseId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            applyChanges(existing);
            existing.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<ExerciseProgress>().PrepareCreate(existing);
        }
        else
        {
            applyChanges(existing);
            existing.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<ExerciseProgress>().PrepareUpdate(existing);
        }

        await _unitOfWork.SaveChangesAsync();
        return existing;
    }

    private CreateExerciseRequest SanitizeCreateRequest(CreateExerciseRequest request, Domain.Enums.ExerciseType exerciseType)
    {
        if (exerciseType == Domain.Enums.ExerciseType.Reading)
        {
            return new CreateExerciseRequest
            {
                ExerciseType = request.ExerciseType,
                Narrative = request.Narrative,
                OrderNumber = request.OrderNumber
            };
        }

        return request;
    }

    private UpdateExerciseRequest SanitizeUpdateRequest(UpdateExerciseRequest request, Domain.Enums.ExerciseType exerciseType)
    {
        if (exerciseType == Domain.Enums.ExerciseType.Reading)
        {
            return new UpdateExerciseRequest
            {
                ExerciseType = request.ExerciseType,
                Narrative = request.Narrative,
                OrderNumber = request.OrderNumber
            };
        }

        return request;
    }

    private async Task<(PistonExecuteResponse? Response, int RuntimeMs)> ExecuteCodeAsync(string language, string code, string? stdin = null)
    {
        var startedAt = DateTime.UtcNow;
        var response = await _pistonService.ExecuteAsync(new PistonExecuteRequest
        {
            Language = language,
            Version = _pistonOptions.Version,
            Stdin = stdin ?? string.Empty,
            Files = new List<PistonFileDto>
            {
                new()
                {
                    Name = GetSourceFileName(language),
                    Content = code
                }
            },
            CompileTimeout = _pistonOptions.CompileTimeout,
            RunTimeout = _pistonOptions.RunTimeout
        });

        var runtimeMs = (int)Math.Max(1, (DateTime.UtcNow - startedAt).TotalMilliseconds);
        return (response, runtimeMs);
    }

    private static string? ResolveLanguage(string? requestedLanguage, string? exerciseLanguage)
    {
        if (!string.IsNullOrWhiteSpace(requestedLanguage))
            return requestedLanguage.Trim();

        if (!string.IsNullOrWhiteSpace(exerciseLanguage))
            return exerciseLanguage.Trim();

        return null;
    }

    private static string NormalizeOutput(string? output)
    {
        if (string.IsNullOrWhiteSpace(output))
            return string.Empty;

        var lines = output
            .Replace("\r\n", "\n")
            .Split('\n', StringSplitOptions.None)
            .Select(line => line.TrimEnd());

        return string.Join("\n", lines).Trim();
    }

    private static string GetSourceFileName(string language)
    {
        var normalized = language.Trim().ToLowerInvariant();
        return normalized switch
        {
            "c" => "main.c",
            "c++" or "cpp" => "main.cpp",
            "c#" or "csharp" or "cs" => "main.cs",
            "go" or "golang" => "main.go",
            "java" => "Main.java",
            "javascript" or "js" => "main.js",
            "typescript" or "ts" => "main.ts",
            "kotlin" or "kt" => "main.kt",
            "php" => "main.php",
            "python" or "py" => "main.py",
            "ruby" or "rb" => "main.rb",
            "rust" or "rs" => "main.rs",
            "swift" => "main.swift",
            _ => "main.txt"
        };
    }

    private static void ApplyExecutionResult(ExerciseProgressDto response, PistonExecuteResponse execution, string language, int runtimeMs)
    {
        response.Language = execution.Language is { Length: > 0 } ? execution.Language : language;
        response.Stdout = execution.Run?.Stdout;
        response.Stderr = execution.Run?.Stderr;
        response.Output = execution.Run?.Output;
        response.ExitCode = execution.Run?.Code;
        response.RuntimeMs = runtimeMs;
        response.CompileStdout = execution.Compile?.Stdout;
        response.CompileStderr = execution.Compile?.Stderr;
        response.CompileOutput = execution.Compile?.Output;
    }
}
