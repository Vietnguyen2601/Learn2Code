using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learn2Code.Application.Services;

public class ProgressService : IProgressService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProgressService> _logger;

    public ProgressService(IUnitOfWork unitOfWork, ILogger<ProgressService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ServiceResult<CourseProgressDto>> GetMyCourseProgressAsync(Guid studentId, Guid courseId)
    {
        try
        {
            // Check enrollment
            var enrollment = await _unitOfWork.EnrollmentRepository
                .GetByStudentAndCourseAsync(studentId, courseId);

            if (enrollment == null)
            {
                return ServiceResult<CourseProgressDto>.Error("NOT_ENROLLED", "You are not enrolled in this course", 403);
            }

            // Get all sections with lessons
            var sections = await _unitOfWork.Repository<Section>()
                .GetAllQueryable()
                .Where(s => s.CourseId == courseId)
                .Include(s => s.Lessons)
                .OrderBy(s => s.OrderNumber)
                .ToListAsync();

            // Get all lesson progresses for this student in this course
            var lessonProgresses = await _unitOfWork.LessonProgressRepository
                .GetByStudentAndCourseAsync(studentId, courseId);

            var sectionProgressDtos = new List<SectionProgressSummaryDto>();

            foreach (var section in sections)
            {
                // Get lesson progresses for this section
                var lessonIds = section.Lessons?.Select(l => l.LessonId).ToList() ?? new List<Guid>();
                var sectionLessonProgresses = lessonProgresses
                    .Where(lp => lessonIds.Contains(lp.LessonId))
                    .ToList();

                // Get best section quiz attempt
                var sectionAttempts = await _unitOfWork.SectionQuizAttemptRepository
                    .GetByStudentAndSectionAsync(studentId, section.SectionId);

                var bestAttempt = sectionAttempts
                    .Where(a => a.IsPassed)
                    .OrderByDescending(a => a.Score)
                    .FirstOrDefault();

                var sectionProgressDto = section.ToSectionProgressSummaryDto(sectionLessonProgresses, bestAttempt);
                sectionProgressDtos.Add(sectionProgressDto);
            }

            // Calculate overall progress percentage
            var totalLessons = sections.Sum(s => s.Lessons?.Count ?? 0);
            var completedLessons = lessonProgresses.Count(lp => lp.Status == LessonProgressStatus.Completed);
            var progressPct = totalLessons > 0 ? (decimal)completedLessons / totalLessons * 100 : 0;

            var result = new CourseProgressDto
            {
                EnrollmentStatus = enrollment.Status,
                ProgressPct = Math.Round(progressPct, 1),
                Sections = sectionProgressDtos
            };

            return ServiceResult<CourseProgressDto>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting course progress for student {StudentId}, course {CourseId}", studentId, courseId);
            return ServiceResult<CourseProgressDto>.Error("ERROR", "An error occurred while retrieving course progress", 500);
        }
    }

    public async Task<ServiceResult<LessonProgressDetailDto>> GetMyLessonProgressAsync(Guid studentId, Guid lessonId)
    {
        try
        {
            // Get lesson with exercises and quizzes
            var lesson = await _unitOfWork.Repository<Lesson>()
                .GetAllQueryable()
                .Include(l => l.Exercises!)
                    .ThenInclude(e => e.TestCases)
                .Include(l => l.Quizzes!)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(l => l.LessonId == lessonId);

            if (lesson == null)
            {
                return ServiceResult<LessonProgressDetailDto>.NotFound("Lesson not found");
            }

            // Check enrollment
            var isEnrolled = await _unitOfWork.EnrollmentRepository
                .IsEnrolledAsync(studentId, lesson.Section.CourseId);

            if (!isEnrolled)
            {
                return ServiceResult<LessonProgressDetailDto>.Error("NOT_ENROLLED", "You are not enrolled in this course", 403);
            }

            // Get lesson progress
            var lessonProgress = await _unitOfWork.Repository<LessonProgress>()
                .GetAsync(lp => lp.StudentId == studentId && lp.LessonId == lessonId);

            // Get exercise progresses
            var exerciseIds = lesson.Exercises?.Select(e => e.ExerciseId).ToList() ?? new List<Guid>();
            var exerciseProgresses = await _unitOfWork.Repository<ExerciseProgress>()
                .GetAllQueryable()
                .Where(ep => ep.StudentId == studentId && exerciseIds.Contains(ep.ExerciseId))
                .ToListAsync();

            var exerciseProgressDtos = new List<ExerciseProgressItemDto>();
            if (lesson.Exercises != null)
            {
                foreach (var exercise in lesson.Exercises.OrderBy(e => e.OrderNumber))
                {
                    var progress = exerciseProgresses.FirstOrDefault(ep => ep.ExerciseId == exercise.ExerciseId);
                    exerciseProgressDtos.Add(exercise.ToExerciseProgressItemDto(progress));
                }
            }

            // Get quiz progress (first quiz in lesson if exists)
            QuizProgressDto? quizProgressDto = null;
            var firstQuiz = lesson.Quizzes?.FirstOrDefault();
            if (firstQuiz != null)
            {
                // Get student's answer from section quiz attempts
                var answer = await _unitOfWork.Repository<SectionQuizAnswer>()
                    .GetAllQueryable()
                    .Include(a => a.Attempt)
                    .FirstOrDefaultAsync(a => a.Attempt.StudentId == studentId && a.QuizId == firstQuiz.QuizId);

                var isAnswered = answer != null;
                bool? isCorrect = answer?.IsCorrect;

                quizProgressDto = firstQuiz.ToQuizProgressDto(isAnswered, isCorrect);
            }

            var result = new LessonProgressDetailDto
            {
                LessonId = lesson.LessonId,
                Title = lesson.Title,
                Status = lessonProgress?.Status ?? LessonProgressStatus.NotStarted,
                Exercises = exerciseProgressDtos,
                Quiz = quizProgressDto
            };

            return ServiceResult<LessonProgressDetailDto>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting lesson progress for student {StudentId}, lesson {LessonId}", studentId, lessonId);
            return ServiceResult<LessonProgressDetailDto>.Error("ERROR", "An error occurred while retrieving lesson progress", 500);
        }
    }

    public async Task<ServiceResult<AllStudentsProgressDto>> GetAllStudentsProgressAsync(Guid courseId)
    {
        try
        {
            // Get course
            var course = await _unitOfWork.Repository<Course>()
                .GetByIdAsync(courseId);

            if (course == null)
            {
                return ServiceResult<AllStudentsProgressDto>.NotFound("Course not found");
            }

            // Get all enrollments with student info
            var enrollments = await _unitOfWork.Repository<Enrollment>()
                .GetAllQueryable()
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();

            // Get total lessons in course
            var totalLessons = await _unitOfWork.Repository<Lesson>()
                .GetAllQueryable()
                .Where(l => l.Section.CourseId == courseId)
                .CountAsync();

            var studentProgressDtos = new List<StudentCourseProgressDto>();

            foreach (var enrollment in enrollments)
            {
                // Get lesson progresses for this student
                var lessonProgresses = await _unitOfWork.LessonProgressRepository
                    .GetByStudentAndCourseAsync(enrollment.StudentId, courseId);

                var completedLessons = lessonProgresses.Count(lp => lp.Status == LessonProgressStatus.Completed);
                var progressPct = totalLessons > 0 ? (decimal)completedLessons / totalLessons * 100 : 0;

                studentProgressDtos.Add(enrollment.ToStudentCourseProgressDto(Math.Round(progressPct, 1)));
            }

            var completedStudents = studentProgressDtos.Count(s => s.EnrollmentStatus == EnrollmentStatus.Completed);
            var avgProgress = studentProgressDtos.Any() ? studentProgressDtos.Average(s => s.ProgressPct) : 0;

            var result = new AllStudentsProgressDto
            {
                CourseId = courseId,
                CourseName = course.Title,
                TotalStudents = enrollments.Count(),
                CompletedStudents = completedStudents,
                AverageProgress = Math.Round(avgProgress, 1),
                Students = studentProgressDtos
            };

            return ServiceResult<AllStudentsProgressDto>.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all students progress for course {CourseId}", courseId);
            return ServiceResult<AllStudentsProgressDto>.Error("ERROR", "An error occurred while retrieving students progress", 500);
        }
    }
}
