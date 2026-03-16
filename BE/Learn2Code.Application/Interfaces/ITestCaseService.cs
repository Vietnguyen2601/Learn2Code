using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;

namespace Learn2Code.Application.Interfaces;

public interface ITestCaseService
{
    Task<ServiceResult<List<TestCaseDto>>> GetTestCasesByExerciseIdAsync(Guid exerciseId, bool isAdmin);
    Task<ServiceResult<TestCaseDto>> CreateTestCaseAsync(Guid exerciseId, CreateTestCaseRequest request);
    Task<ServiceResult<TestCaseDto>> UpdateTestCaseAsync(Guid exerciseId, Guid testCaseId, UpdateTestCaseRequest request);
    Task<ServiceResult> DeleteTestCaseAsync(Guid exerciseId, Guid testCaseId);
}
