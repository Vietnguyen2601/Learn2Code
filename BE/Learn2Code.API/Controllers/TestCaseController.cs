using System.Security.Claims;
using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/exercises")]
public class TestCaseController : ControllerBase
{
    private readonly ITestCaseService _testCaseService;

    public TestCaseController(ITestCaseService testCaseService)
    {
        _testCaseService = testCaseService;
    }

    /// <summary>
    /// Get all test cases of an exercise (Admin sees all, Student sees non-hidden only)
    /// </summary>
    [HttpGet("{exerciseId}/testcases")]
    [Authorize]
    [ProducesResponseType(typeof(ServiceResult<List<TestCaseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<List<TestCaseDto>>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTestCasesByExercise(Guid exerciseId)
    {
        // Check if user is Admin
        var isAdmin = User.IsInRole("Admin");

        var result = await _testCaseService.GetTestCasesByExerciseIdAsync(exerciseId, isAdmin);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Create a new test case for exercise (Admin only - GradedCode exercises only)
    /// </summary>
    [HttpPost("{exerciseId}/testcases")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<TestCaseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResult<TestCaseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<TestCaseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateTestCase(Guid exerciseId, [FromBody] CreateTestCaseRequest request)
    {
        var result = await _testCaseService.CreateTestCaseAsync(exerciseId, request);
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }

    /// <summary>
    /// Update a test case (Admin only)
    /// </summary>
    [HttpPatch("{exerciseId}/testcases/{testcaseId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<TestCaseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<TestCaseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<TestCaseDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateTestCase(Guid exerciseId, Guid testcaseId, [FromBody] UpdateTestCaseRequest request)
    {
        var result = await _testCaseService.UpdateTestCaseAsync(exerciseId, testcaseId, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Delete a test case (Admin only)
    /// </summary>
    [HttpDelete("{exerciseId}/testcases/{testcaseId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteTestCase(Guid exerciseId, Guid testcaseId)
    {
        var result = await _testCaseService.DeleteTestCaseAsync(exerciseId, testcaseId);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
