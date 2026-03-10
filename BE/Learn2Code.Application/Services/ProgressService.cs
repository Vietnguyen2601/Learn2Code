using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Learn2Code.Application.Services;

public class ProgressService : IProgressService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProgressService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<CourseProgressDto>> GetCourseProgressAsync(Guid courseId, Guid studentId)
    {
        // Kiểm tra course có tồn tại không
        var course = await _unitOfWork.CourseRepository.GetByIdAsync(courseId);
        if (course == null)
            return ServiceResult<CourseProgressDto>.NotFound("Course not found");

        // Kiểm tra enrollment
        var enrollment = await _unitOfWork.EnrollmentRepository
            .GetEnrollmentByStudentAndCourseAsync(studentId, courseId);
        if (enrollment == null)
            return ServiceResult<CourseProgressDto>.Error("NOT_ENROLLED", "You are not enrolled in this course", 403);

        // Lấy tất cả sections của course
        var sections = await _unitOfWork.SectionRepository.GetByCourseIdAsync(courseId);
        if (!sections.Any())
        {
            return ServiceResult<CourseProgressDto>.Ok(new CourseProgressDto
            {
                EnrollmentStatus = enrollment.Status.ToString(),
                ProgressPct = 0,
                Sections = new List<SectionProgressDto>()
            });
        }

        var sectionProgressDtos = new List<SectionProgressDto>();
        var totalLessons = 0;
        var totalCompletedLessons = 0;

        foreach (var section in sections.OrderBy(s => s.OrderNumber))
        {
            // Lấy tất cả lessons trong section
            var lessons = await _unitOfWork.LessonRepository.GetLessonsBySectionIdAsync(section.SectionId);
            totalLessons += lessons.Count();

            // Lấy lesson progresses
            var lessonIds = lessons.Select(l => l.LessonId).ToList();
            var lessonProgresses = await _unitOfWork.Repository<LessonProgress>()
                .GetAllQueryable()
                .Where(lp => lp.StudentId == studentId && lessonIds.Contains(lp.LessonId))
                .ToListAsync();

            var completedCount = lessonProgresses.Count(lp => lp.Status == LessonProgressStatus.Completed);
            totalCompletedLessons += completedCount;

            // Check section quiz unlocked
            var sectionQuizUnlocked = completedCount >= lessons.Count();

            // Lấy best attempt của section quiz
            var bestAttempt = await _unitOfWork.Repository<SectionQuizAttempt>()
                .GetAllQueryable()
                .Where(a => a.SectionId == section.SectionId && a.StudentId == studentId)
                .OrderByDescending(a => a.Score)
                .ToListAsync();

            var sectionQuizPassed = bestAttempt.Any() && bestAttempt.First().IsPassed;
            var sectionQuizScore = bestAttempt.Any() ? (decimal?)bestAttempt.First().Score : null;

            sectionProgressDtos.Add(new SectionProgressDto
            {
                SectionId = section.SectionId,
                Title = section.Title,
                LessonsTotal = lessons.Count(),
                LessonsCompleted = completedCount,
                SectionQuizUnlocked = sectionQuizUnlocked,
                SectionQuizPassed = sectionQuizPassed,
                SectionQuizScore = sectionQuizScore
            });
        }

        var progressPct = totalLessons > 0 
            ? Math.Round((decimal)totalCompletedLessons / totalLessons * 100, 2) 
            : 0;

        var courseProgressDto = new CourseProgressDto
        {
            EnrollmentStatus = enrollment.Status.ToString(),
            ProgressPct = progressPct,
            Sections = sectionProgressDtos
        };

        return ServiceResult<CourseProgressDto>.Ok(courseProgressDto);
    }

    public async Task<ServiceResult<LessonProgressDetailDto>> GetLessonProgressAsync(Guid lessonId, Guid studentId)
    {
        // Kiểm tra lesson có tồn tại không
        var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
            return ServiceResult<LessonProgressDetailDto>.NotFound("Lesson not found");

        // Kiểm tra quyền truy cập
        var canAccess = await _unitOfWork.LessonRepository.CanUserAccessLessonAsync(lessonId, studentId);
        if (!canAccess)
            return ServiceResult<LessonProgressDetailDto>.Error("ACCESS_DENIED", "You don't have permission to access this lesson", 403);

        // Lấy lesson progress
        var lessonProgress = await _unitOfWork.Repository<LessonProgress>()
            .GetAsync(lp => lp.LessonId == lessonId && lp.StudentId == studentId);

        var status = lessonProgress?.Status.ToString() ?? LessonProgressStatus.NotStarted.ToString();

        // Lấy tất cả exercises trong lesson
        var exercises = await _unitOfWork.ExerciseRepository.GetExercisesByLessonIdAsync(lessonId);
        var exerciseIds = exercises.Select(e => e.ExerciseId).ToList();

        // Lấy exercise progresses
        var exerciseProgresses = await _unitOfWork.Repository<ExerciseProgress>()
            .GetAllQueryable()
            .Where(ep => ep.StudentId == studentId && exerciseIds.Contains(ep.ExerciseId))
            .ToListAsync();

        var exerciseProgressDict = exerciseProgresses.ToDictionary(ep => ep.ExerciseId);

        var exerciseSummaries = exercises.OrderBy(e => e.OrderNumber).Select(e =>
        {
            var hasProgress = exerciseProgressDict.TryGetValue(e.ExerciseId, out var progress);
            return new ExerciseProgressSummaryDto
            {
                ExerciseId = e.ExerciseId,
                ExerciseType = e.ExerciseType.ToString(),
                OrderNumber = e.OrderNumber,
                IsCompleted = hasProgress && progress.IsCompleted,
                IsPassed = hasProgress && progress.IsPassed
            };
        }).ToList();

        var detailDto = new LessonProgressDetailDto
        {
            LessonId = lessonId,
            LessonTitle = lesson.Title,
            Status = status,
            Exercises = exerciseSummaries
        };

        return ServiceResult<LessonProgressDetailDto>.Ok(detailDto);
    }

    public async Task<ServiceResult<LessonProgressDto>> UpdateLessonProgressAsync(
        Guid lessonId, Guid studentId, UpdateLessonProgressRequest request)
    {
        // Kiểm tra lesson có tồn tại không
        var lesson = await _unitOfWork.LessonRepository.GetByIdAsync(lessonId);
        if (lesson == null)
            return ServiceResult<LessonProgressDto>.NotFound("Lesson not found");

        // Validate status
        if (!Enum.TryParse<LessonProgressStatus>(request.Status, true, out var newStatus))
            return ServiceResult<LessonProgressDto>.Error("INVALID_STATUS", "Status must be one of: NotStarted, InProgress, Completed", 400);

        // Lấy hoặc tạo lesson progress
        var lessonProgress = await _unitOfWork.Repository<LessonProgress>()
            .GetAsync(lp => lp.LessonId == lessonId && lp.StudentId == studentId);

        if (lessonProgress == null)
        {
            lessonProgress = new LessonProgress
            {
                ProgressId = Guid.NewGuid(),
                StudentId = studentId,
                LessonId = lessonId,
                Status = newStatus,
                LastAccessedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (newStatus == LessonProgressStatus.Completed)
            {
                lessonProgress.CompletedAt = DateTime.UtcNow;
            }

            _unitOfWork.Repository<LessonProgress>().PrepareCreate(lessonProgress);
        }
        else
        {
            lessonProgress.Status = newStatus;
            lessonProgress.LastAccessedAt = DateTime.UtcNow;
            lessonProgress.UpdatedAt = DateTime.UtcNow;

            if (newStatus == LessonProgressStatus.Completed && lessonProgress.CompletedAt == null)
            {
                lessonProgress.CompletedAt = DateTime.UtcNow;
            }
            else if (newStatus != LessonProgressStatus.Completed)
            {
                lessonProgress.CompletedAt = null;
            }

            _unitOfWork.Repository<LessonProgress>().PrepareUpdate(lessonProgress);
        }

        await _unitOfWork.SaveChangesAsync();

        var dto = lessonProgress.ToDto();
        return ServiceResult<LessonProgressDto>.Ok(dto, "Lesson progress updated successfully");
    }
}
