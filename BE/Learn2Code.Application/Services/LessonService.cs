using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;

namespace Learn2Code.Application.Services;

public class LessonService : ILessonService
{
    private readonly IUnitOfWork _unitOfWork;

    public LessonService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<LessonDetailDto>> GetLessonAsync(Guid lessonId, Guid studentId)
    {
        // Get lesson with exercises
        var lesson = await _unitOfWork.LessonRepository.GetWithExercisesAsync(lessonId);
        
        if (lesson == null)
        {
            return ServiceResult<LessonDetailDto>.NotFound("Lesson not found");
        }

        // Get section to find course
        var section = await _unitOfWork.Repository<Domain.Entities.Section>()
            .GetByIdAsync(lesson.SectionId);
        
        if (section == null)
        {
            return ServiceResult<LessonDetailDto>.NotFound("Section not found");
        }

        var courseId = section.CourseId;
        bool isAccessible = false;
        string? accessMessage = null;

        // Check 1: Is this a free preview lesson?
        if (lesson.IsFreePreview)
        {
            isAccessible = true;
        }
        else
        {
            // Check 2: Is student enrolled?
            var enrollment = await _unitOfWork.EnrollmentRepository
                .GetByStudentAndCourseAsync(studentId, courseId);

            if (enrollment == null)
            {
                accessMessage = "You must enroll in this course to access this lesson";
            }
            else
            {
                // Check 3: Does student have active subscription?
                var activeSubscription = await _unitOfWork.Repository<Domain.Entities.UserSubscription>()
                    .GetAsync(us => us.UserId == studentId && us.Status == SubscriptionStatus.Active);

                if (activeSubscription == null)
                {
                    accessMessage = "You need an active subscription to access this lesson";
                }
                else
                {
                    // Check 4: Is lesson locked? (Check previous lessons)
                    var allLessonsInSection = await _unitOfWork.LessonRepository
                        .GetBySectionIdAsync(lesson.SectionId);
                    
                    var previousLessons = allLessonsInSection
                        .Where(l => l.OrderNumber < lesson.OrderNumber)
                        .OrderBy(l => l.OrderNumber)
                        .ToList();

                    if (previousLessons.Any())
                    {
                        // Check if all previous lessons are completed
                        var lessonProgresses = await _unitOfWork.LessonProgressRepository
                            .GetByStudentAndCourseAsync(studentId, courseId);

                        var allPreviousCompleted = previousLessons.All(pl =>
                            lessonProgresses.Any(lp => 
                                lp.LessonId == pl.LessonId && 
                                lp.Status == LessonProgressStatus.Completed));

                        if (!allPreviousCompleted)
                        {
                            accessMessage = "You must complete previous lessons first";
                        }
                        else
                        {
                            isAccessible = true;
                        }
                    }
                    else
                    {
                        // First lesson in section, always accessible
                        isAccessible = true;
                    }
                }
            }
        }

        // Get exercise progresses
        var exerciseProgresses = await _unitOfWork.ExerciseProgressRepository
            .GetByStudentAndLessonAsync(studentId, lessonId);

        // Get lesson progress
        var lessonProgress = await _unitOfWork.LessonProgressRepository
            .GetByStudentAndLessonAsync(studentId, lessonId);

        // Update last accessed time if accessible
        if (isAccessible)
        {
            if (lessonProgress == null)
            {
                lessonProgress = new Domain.Entities.LessonProgress
                {
                    ProgressId = Guid.NewGuid(),
                    StudentId = studentId,
                    LessonId = lessonId,
                    Status = LessonProgressStatus.InProgress,
                    LastAccessedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _unitOfWork.LessonProgressRepository.PrepareCreate(lessonProgress);
            }
            else
            {
                lessonProgress.LastAccessedAt = DateTime.UtcNow;
                lessonProgress.UpdatedAt = DateTime.UtcNow;
                if (lessonProgress.Status == LessonProgressStatus.NotStarted)
                {
                    lessonProgress.Status = LessonProgressStatus.InProgress;
                }
                _unitOfWork.LessonProgressRepository.PrepareUpdate(lessonProgress);
            }

            // Update ActivatedAt on first lesson access (when student starts learning for the first time)
            var enrollment = await _unitOfWork.EnrollmentRepository
                .GetByStudentAndCourseAsync(studentId, courseId);
            
            if (enrollment != null && enrollment.ActivatedAt == null)
            {
                enrollment.ActivatedAt = DateTime.UtcNow;
                _unitOfWork.EnrollmentRepository.PrepareUpdate(enrollment);
            }

            await _unitOfWork.CommitTransactionAsync();
        }

        var exercises = lesson.Exercises.ToList();
        var lessonDetailDto = lesson.ToLessonDetailDto(
            isAccessible, 
            accessMessage, 
            exercises, 
            exerciseProgresses, 
            lessonProgress);

        return ServiceResult<LessonDetailDto>.Ok(lessonDetailDto);
    }
}
