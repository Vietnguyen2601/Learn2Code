using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;

namespace Learn2Code.Application.Interfaces;

public interface IExerciseService
{
    /// <summary>
    /// Run code for FreeCode exercise (no grading)
    /// </summary>
    Task<ServiceResult<RunCodeResponse>> RunCodeAsync(Guid exerciseId, Guid studentId, RunCodeRequest request);

    /// <summary>
    /// Submit code for GradedCode exercise (with test cases grading)
    /// </summary>
    Task<ServiceResult<SubmitCodeResponse>> SubmitCodeAsync(Guid exerciseId, Guid studentId, SubmitCodeRequest request);

    /// <summary>
    /// Update exercise progress (auto-save last_code, mark completed)
    /// </summary>
    Task<ServiceResult<ExerciseProgressDto>> UpdateProgressAsync(Guid exerciseId, Guid studentId, UpdateProgressRequest request);
}
