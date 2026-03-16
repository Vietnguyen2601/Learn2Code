using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Application.Services;

public class CertificationService : ICertificationService
{
    private readonly IUnitOfWork _unitOfWork;

    public CertificationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<List<CertificationDto>>> GetMyCertificationsAsync(Guid studentId)
    {
        var certifications = await _unitOfWork.CertificationRepository.GetByStudentIdAsync(studentId);
        var dtos = certifications.Select(c => c.ToDto()).ToList();
        return ServiceResult<List<CertificationDto>>.Ok(dtos);
    }

    public async Task<ServiceResult<CertificateVerificationDto>> VerifyCertificateAsync(string certificateCode)
    {
        if (string.IsNullOrWhiteSpace(certificateCode))
        {
            return ServiceResult<CertificateVerificationDto>.BadRequest("Certificate code is required");
        }

        var certification = await _unitOfWork.CertificationRepository.GetByCertificateCodeAsync(certificateCode);
        var dto = certification.ToVerificationDto(certificateCode);
        
        return ServiceResult<CertificateVerificationDto>.Ok(dto);
    }

    public async Task<ServiceResult<List<CertificationDto>>> GetAllCertificationsAsync()
    {
        var certifications = await _unitOfWork.CertificationRepository.GetAllWithDetailsAsync();
        var dtos = certifications.Select(c => c.ToDto()).ToList();
        return ServiceResult<List<CertificationDto>>.Ok(dtos);
    }

    public async Task<ServiceResult<CertificationEligibilityDto>> CheckCertificationEligibilityAsync(Guid studentId, Guid courseId)
    {
        // Check if course exists
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            return ServiceResult<CertificationEligibilityDto>.NotFound("Course not found");
        }

        // Check if student is enrolled
        var enrollment = await _unitOfWork.EnrollmentRepository.GetEnrollmentByStudentAndCourseAsync(studentId, courseId);
        if (enrollment == null)
        {
            return ServiceResult<CertificationEligibilityDto>.Error("NOT_ENROLLED", "You are not enrolled in this course");
        }

        // Check if already certified
        var existingCertification = await _unitOfWork.CertificationRepository.GetByStudentAndCourseAsync(studentId, courseId);
        if (existingCertification != null)
        {
            return ServiceResult<CertificationEligibilityDto>.Ok(new CertificationEligibilityDto
            {
                IsEligible = true,
                AlreadyCertified = true,
                ExistingCertificateCode = existingCertification.CertificateCode,
                Progress = new CertificationProgressDto(),
                Requirements = new CertificationRequirementsDto()
            });
        }

        // Get completion rules
        var rule = await _unitOfWork.Repository<CourseCompletionRule>()
            .GetAsync(r => r.CourseId == courseId);

        // Calculate progress and check eligibility
        var (progress, missing) = await CalculateProgressAsync(studentId, courseId, rule);
        var requirements = rule.ToRequirementsDto();

        var isEligible = missing.Count == 0;

        return ServiceResult<CertificationEligibilityDto>.Ok(new CertificationEligibilityDto
        {
            IsEligible = isEligible,
            AlreadyCertified = false,
            Progress = progress,
            Requirements = requirements,
            Missing = missing
        });
    }

    public async Task<ServiceResult<IssueCertificationResultDto>> TryIssueCertificateAsync(Guid studentId, Guid courseId)
    {
        // Check eligibility first
        var eligibilityResult = await CheckCertificationEligibilityAsync(studentId, courseId);
        if (!eligibilityResult.Success)
        {
            return ServiceResult<IssueCertificationResultDto>.Error(
                eligibilityResult.ErrorCode ?? "ERROR",
                eligibilityResult.Message ?? "Failed to check eligibility",
                eligibilityResult.Status);
        }

        var eligibility = eligibilityResult.Data!;

        // Already certified
        if (eligibility.AlreadyCertified)
        {
            return ServiceResult<IssueCertificationResultDto>.Ok(new IssueCertificationResultDto
            {
                Certified = true,
                CertificateCode = eligibility.ExistingCertificateCode,
                Message = "You already have a certificate for this course"
            });
        }

        // Not eligible
        if (!eligibility.IsEligible)
        {
            return ServiceResult<IssueCertificationResultDto>.Ok(new IssueCertificationResultDto
            {
                Certified = false,
                Missing = eligibility.Missing,
                Message = "You have not met all requirements for certification"
            });
        }

        // Issue certificate
        var certification = new Certification
        {
            CertificationId = Guid.NewGuid(),
            StudentId = studentId,
            CourseId = courseId,
            CertificateCode = CertificationMapper.GenerateCertificateCode(),
            CertificateUrl = null, // Can be set later when PDF is generated
            IssuedAt = DateTime.UtcNow
        };

        _unitOfWork.CertificationRepository.PrepareCreate(certification);

        // Update enrollment status to Completed
        var enrollment = await _unitOfWork.EnrollmentRepository.GetEnrollmentByStudentAndCourseAsync(studentId, courseId);
        if (enrollment != null)
        {
            enrollment.Status = EnrollmentStatus.Completed;
            enrollment.ProgressPct = 100;
            enrollment.CompletedAt = DateTime.UtcNow;
            _unitOfWork.EnrollmentRepository.PrepareUpdate(enrollment);
        }

        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<IssueCertificationResultDto>.Created(new IssueCertificationResultDto
        {
            Certified = true,
            CertificateCode = certification.CertificateCode,
            CertificateUrl = certification.CertificateUrl,
            Message = "Congratulations! Certificate issued successfully"
        }, "Certificate issued successfully");
    }

    /// <summary>
    /// Calculate student's progress toward certification
    /// </summary>
    private async Task<(CertificationProgressDto progress, List<string> missing)> CalculateProgressAsync(
        Guid studentId, 
        Guid courseId, 
        CourseCompletionRule? rule)
    {
        var missing = new List<string>();
        var progress = new CertificationProgressDto();

        // Default requirements if no rule exists
        var minLessonPct = rule?.MinLessonCompletionPct ?? 100;
        var minExercisePct = rule?.MinExercisePassPct ?? 0;
        var minQuizScore = rule?.MinSectionQuizScore ?? 0;
        var requireAllQuiz = rule?.RequireAllSectionQuiz ?? false;

        // Get all sections of the course
        var sections = await _unitOfWork.SectionRepository.GetAllQueryable()
            .Where(s => s.CourseId == courseId && s.IsActive)
            .Include(s => s.Lessons)
            .ToListAsync();

        var allLessonIds = sections.SelectMany(s => s.Lessons.Select(l => l.LessonId)).ToList();
        var sectionIds = sections.Select(s => s.SectionId).ToList();

        // 1. Calculate Lesson Completion Percentage
        if (allLessonIds.Count > 0)
        {
            var completedLessons = await _unitOfWork.Repository<LessonProgress>().GetAllQueryable()
                .Where(lp => lp.StudentId == studentId && 
                             allLessonIds.Contains(lp.LessonId) && 
                             lp.Status == LessonProgressStatus.Completed)
                .CountAsync();

            progress.LessonCompletionPct = Math.Round((decimal)completedLessons / allLessonIds.Count * 100, 2);
        }
        else
        {
            progress.LessonCompletionPct = 100; // No lessons = 100% complete
        }

        if (progress.LessonCompletionPct < minLessonPct)
        {
            missing.Add($"Lesson completion: {progress.LessonCompletionPct}% (required: {minLessonPct}%)");
        }

        // 2. Calculate Exercise Pass Percentage
        var allExerciseIds = await _unitOfWork.ExerciseRepository.GetAllQueryable()
            .Where(e => allLessonIds.Contains(e.LessonId))
            .Select(e => e.ExerciseId)
            .ToListAsync();

        if (allExerciseIds.Count > 0)
        {
            var passedExercises = await _unitOfWork.Repository<ExerciseProgress>().GetAllQueryable()
                .Where(ep => ep.StudentId == studentId && 
                             allExerciseIds.Contains(ep.ExerciseId) && 
                             ep.IsPassed)
                .CountAsync();

            progress.ExercisePassPct = Math.Round((decimal)passedExercises / allExerciseIds.Count * 100, 2);
        }
        else
        {
            progress.ExercisePassPct = 100; // No exercises = 100% pass
        }

        if (progress.ExercisePassPct < minExercisePct)
        {
            missing.Add($"Exercise pass rate: {progress.ExercisePassPct}% (required: {minExercisePct}%)");
        }

        // 3. Calculate Section Quiz Score (average of best attempts per section)
        // Get sections that have quizzes (sections where student has at least one quiz to attempt)
        var sectionsWithQuizzes = await _unitOfWork.Repository<Quiz>().GetAllQueryable()
            .Where(q => sections.SelectMany(s => s.Lessons.Select(l => l.LessonId)).Contains(q.LessonId))
            .Select(q => q.Lesson.SectionId)
            .Distinct()
            .ToListAsync();

        progress.TotalSectionsWithQuiz = sectionsWithQuizzes.Count;

        if (sectionsWithQuizzes.Count > 0)
        {
            // Get best score per section for this student
            var quizAttempts = await _unitOfWork.Repository<SectionQuizAttempt>().GetAllQueryable()
                .Where(qa => qa.StudentId == studentId && sectionIds.Contains(qa.SectionId))
                .ToListAsync();

            var sectionsAttempted = quizAttempts
                .GroupBy(qa => qa.SectionId)
                .Select(g => new { SectionId = g.Key, BestScore = g.Max(qa => qa.Score) })
                .ToList();

            progress.SectionsWithQuizAttempt = sectionsAttempted.Count;

            if (sectionsAttempted.Count > 0)
            {
                progress.SectionQuizAvgScore = Math.Round(sectionsAttempted.Average(s => s.BestScore), 2);
            }

            // Check min quiz score
            if (progress.SectionQuizAvgScore < minQuizScore)
            {
                missing.Add($"Section quiz average score: {progress.SectionQuizAvgScore}% (required: {minQuizScore}%)");
            }

            // Check if all sections with quizzes are attempted
            if (requireAllQuiz && progress.SectionsWithQuizAttempt < progress.TotalSectionsWithQuiz)
            {
                var notAttemptedCount = progress.TotalSectionsWithQuiz - progress.SectionsWithQuizAttempt;
                missing.Add($"Section quizzes not attempted: {notAttemptedCount} section(s)");
            }
        }

        return (progress, missing);
    }

    public async Task<ServiceResult<List<CertificateTemplateDto>>> GetAllCertificateTemplatesAsync()
    {
        var templates = await _unitOfWork.Repository<CertificateTemplate>()
            .GetAllQueryable()
            .Include(t => t.Course)
            .OrderBy(t => t.Course.Title)
            .ToListAsync();

        var dtos = templates.Select(t => t.ToTemplateDto()).ToList();
        return ServiceResult<List<CertificateTemplateDto>>.Ok(dtos);
    }

    public async Task<ServiceResult<CertificateTemplateDto>> GetCertificateTemplateByCourseIdAsync(Guid courseId)
    {
        var template = await _unitOfWork.Repository<CertificateTemplate>()
            .GetAllQueryable()
            .Include(t => t.Course)
            .FirstOrDefaultAsync(t => t.CourseId == courseId);

        if (template == null)
            return ServiceResult<CertificateTemplateDto>.NotFound("Certificate template not found for this course");

        return ServiceResult<CertificateTemplateDto>.Ok(template.ToTemplateDto());
    }
}
