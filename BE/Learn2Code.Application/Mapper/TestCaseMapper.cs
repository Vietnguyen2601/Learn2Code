using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;

namespace Learn2Code.Application.Mapper;

public static class TestCaseMapper
{
    public static TestCaseDto ToDto(this TestCase testCase)
    {
        return new TestCaseDto
        {
            TestCaseId = testCase.TestCaseId,
            ExerciseId = testCase.ExerciseId,
            ExpectedOutput = testCase.ExpectedOutput,
            IsHidden = testCase.IsHidden,
            Weight = testCase.Weight,
            CreatedAt = testCase.CreatedAt,
            UpdatedAt = testCase.UpdatedAt
        };
    }

    public static TestCase ToEntity(this CreateTestCaseRequest request, Guid exerciseId)
    {
        return new TestCase
        {
            TestCaseId = Guid.NewGuid(),
            ExerciseId = exerciseId,
            ExpectedOutput = request.ExpectedOutput,
            IsHidden = request.IsHidden,
            Weight = request.Weight,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateTestCase(this TestCase testCase, UpdateTestCaseRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.ExpectedOutput))
            testCase.ExpectedOutput = request.ExpectedOutput;

        if (request.IsHidden.HasValue)
            testCase.IsHidden = request.IsHidden.Value;

        if (request.Weight.HasValue)
            testCase.Weight = request.Weight.Value;

        testCase.UpdatedAt = DateTime.UtcNow;
    }
}
