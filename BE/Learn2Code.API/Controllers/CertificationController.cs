using System.Security.Claims;
using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.CertificationDTOs.CertificationResponses;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/certifications")]
public class CertificationController : ControllerBase
{
    private readonly ICertificationService _certificationService;

    public CertificationController(ICertificationService certificationService)
    {
        _certificationService = certificationService;
    }

    /// <summary>
    /// Get all certifications for the current student [Student,Admin]
    /// Admin can use ?studentId= query param to view student's certifications
    /// </summary>
    [HttpGet("me")]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(ServiceResult<List<CertificationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMyCertifications([FromQuery] Guid? studentId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ServiceResult.Error("UNAUTHORIZED", "Invalid token"));

        // Admin can view other student's certifications with ?studentId=
        var targetStudentId = studentId ?? userId.Value;
        
        if (!User.IsInRole("Admin") && studentId.HasValue && studentId.Value != userId.Value)
        {
            return Forbid();
        }

        var result = await _certificationService.GetMyCertificationsAsync(targetStudentId);
        return Ok(result);
    }

    /// <summary>
    /// Verify/view certificate by code [Public]
    /// </summary>
    /// <param name="code">Certificate code (e.g., L2C-202506-A1B2C3)</param>
    [HttpGet("{code}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ServiceResult<CertificateVerificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<CertificateVerificationDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyCertificate(string code)
    {
        var result = await _certificationService.VerifyCertificateAsync(code);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get all certifications [Admin only]
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<List<CertificationDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllCertifications()
    {
        var result = await _certificationService.GetAllCertificationsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Get all certificate templates [Admin only]
    /// </summary>
    [HttpGet("templates")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<List<CertificateTemplateDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllCertificateTemplates()
    {
        var result = await _certificationService.GetAllCertificateTemplatesAsync();
        return Ok(result);
    }

    /// <summary>
    /// Get certificate template by course ID [Admin only]
    /// </summary>
    /// <param name="courseId">Course ID</param>
    [HttpGet("templates/course/{courseId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceResult<CertificateTemplateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<CertificateTemplateDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCertificateTemplateByCourse(Guid courseId)
    {
        var result = await _certificationService.GetCertificateTemplateByCourseIdAsync(courseId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Get current user ID from JWT claims
    /// </summary>
    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                          ?? User.FindFirst("sub")?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return null;

        return userId;
    }
}

/// <summary>
/// Certification check endpoint under courses
/// </summary>
[ApiController]
[Route("api/courses/{courseId}/certifications")]
public class CourseCertificationController : ControllerBase
{
    private readonly ICertificationService _certificationService;

    public CourseCertificationController(ICertificationService certificationService)
    {
        _certificationService = certificationService;
    }

    /// <summary>
    /// Check if current student is eligible for certification [Student,Admin]
    /// Admin can use ?studentId= query param to check student's eligibility
    /// </summary>
    /// <param name="courseId">Course ID</param>
    [HttpPost("check")]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(ServiceResult<CertificationEligibilityDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<CertificationEligibilityDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<CertificationEligibilityDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CheckEligibility(Guid courseId, [FromQuery] Guid? studentId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ServiceResult.Error("UNAUTHORIZED", "Invalid token"));

        // Admin can check other student's eligibility with ?studentId=
        var targetStudentId = studentId ?? userId.Value;
        
        if (!User.IsInRole("Admin") && studentId.HasValue && studentId.Value != userId.Value)
        {
            return Forbid();
        }

        var result = await _certificationService.CheckCertificationEligibilityAsync(targetStudentId, courseId);
        
        if (!result.Success)
        {
            return result.Status == 404 ? NotFound(result) : BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Request to issue certificate (if eligible) [Student,Admin]
    /// Admin can use ?studentId= query param to issue certificate for student
    /// </summary>
    /// <param name="courseId">Course ID</param>
    [HttpPost("issue")]
    [Authorize(Roles = "Student,Admin")]
    [ProducesResponseType(typeof(ServiceResult<IssueCertificationResultDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<IssueCertificationResultDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ServiceResult<IssueCertificationResultDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ServiceResult<IssueCertificationResultDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> IssueCertificate(Guid courseId, [FromQuery] Guid? studentId)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(ServiceResult.Error("UNAUTHORIZED", "Invalid token"));

        // Admin can issue certificate for other student with ?studentId=
        var targetStudentId = studentId ?? userId.Value;
        
        if (!User.IsInRole("Admin") && studentId.HasValue && studentId.Value != userId.Value)
        {
            return Forbid();
        }

        var result = await _certificationService.TryIssueCertificateAsync(targetStudentId, courseId);
        
        if (!result.Success)
        {
            return result.Status == 404 ? NotFound(result) : BadRequest(result);
        }

        // Return 201 if newly created, 200 if already existed or not eligible
        return result.Status == 201 ? StatusCode(201, result) : Ok(result);
    }

    /// <summary>
    /// Get current user ID from JWT claims
    /// </summary>
    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                          ?? User.FindFirst("sub")?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return null;

        return userId;
    }
}
