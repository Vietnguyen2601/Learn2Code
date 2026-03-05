using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;

namespace Learn2Code.Application.Services;

public class TestCaseService : ITestCaseService
{
    private readonly IUnitOfWork _unitOfWork;

    public TestCaseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<List<TestCaseDto>>> GetTestCasesByExerciseIdAsync(Guid exerciseId, bool isAdmin)
    {
        // Ki?m tra exercise có t?n t?i không
        var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(exerciseId);
        if (exercise == null)
            return ServiceResult<List<TestCaseDto>>.NotFound("Exercise not found");

        var testCases = await _unitOfWork.TestCaseRepository.GetTestCasesByExerciseIdAsync(exerciseId);

        // N?u không ph?i Admin, ch? tr? v? test cases không hidden
        if (!isAdmin)
        {
            testCases = testCases.Where(tc => !tc.IsHidden).ToList();
        }

        var testCaseDtos = testCases.Select(tc => tc.ToDto()).ToList();

        return ServiceResult<List<TestCaseDto>>.Ok(testCaseDtos);
    }

    public async Task<ServiceResult<TestCaseDto>> CreateTestCaseAsync(Guid exerciseId, CreateTestCaseRequest request)
    {
        // Ki?m tra exercise có t?n t?i không
        var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(exerciseId);
        if (exercise == null)
            return ServiceResult<TestCaseDto>.NotFound("Exercise not found");

        // Ki?m tra exercise ph?i là GradedCode m?i ???c thêm test case
        if (exercise.ExerciseType != ExerciseType.GradedCode)
            return ServiceResult<TestCaseDto>.Error("INVALID_EXERCISE_TYPE", "Test cases can only be added to GradedCode exercises");

        var testCase = request.ToEntity(exerciseId);
        _unitOfWork.TestCaseRepository.PrepareCreate(testCase);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<TestCaseDto>.Created(testCase.ToDto(), "Test case created successfully");
    }

    public async Task<ServiceResult<TestCaseDto>> UpdateTestCaseAsync(Guid exerciseId, Guid testCaseId, UpdateTestCaseRequest request)
    {
        // Ki?m tra exercise có t?n t?i không
        var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(exerciseId);
        if (exercise == null)
            return ServiceResult<TestCaseDto>.NotFound("Exercise not found");

        // Ki?m tra test case có t?n t?i và thu?c exercise này không
        var exists = await _unitOfWork.TestCaseRepository.ExistsInExerciseAsync(exerciseId, testCaseId);
        if (!exists)
            return ServiceResult<TestCaseDto>.NotFound("Test case not found or does not belong to this exercise");

        var testCase = await _unitOfWork.TestCaseRepository.GetByIdAsync(testCaseId);
        if (testCase == null)
            return ServiceResult<TestCaseDto>.NotFound("Test case not found");

        testCase.UpdateTestCase(request);
        _unitOfWork.TestCaseRepository.PrepareUpdate(testCase);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<TestCaseDto>.Ok(testCase.ToDto(), "Test case updated successfully");
    }

    public async Task<ServiceResult> DeleteTestCaseAsync(Guid exerciseId, Guid testCaseId)
    {
        // Ki?m tra exercise có t?n t?i không
        var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(exerciseId);
        if (exercise == null)
            return ServiceResult.NotFound("Exercise not found");

        // Ki?m tra test case có t?n t?i và thu?c exercise này không
        var exists = await _unitOfWork.TestCaseRepository.ExistsInExerciseAsync(exerciseId, testCaseId);
        if (!exists)
            return ServiceResult.NotFound("Test case not found or does not belong to this exercise");

        var testCase = await _unitOfWork.TestCaseRepository.GetByIdAsync(testCaseId);
        if (testCase == null)
            return ServiceResult.NotFound("Test case not found");

        _unitOfWork.TestCaseRepository.PrepareRemove(testCase);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Ok("Test case deleted successfully");
    }
}
