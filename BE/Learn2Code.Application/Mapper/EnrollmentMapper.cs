using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Application.Mapper;

public static class EnrollmentMapper
{
    public static EnrollmentDto ToEnrollmentDto(this Enrollment enrollment)
    {
        return new EnrollmentDto
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            CourseId = enrollment.CourseId,
            CourseTitle = enrollment.Course?.Title ?? string.Empty,
            Status = enrollment.Status.ToString(),
            ProgressPct = enrollment.ProgressPct,
            EnrolledAt = enrollment.EnrolledAt,
            CompletedAt = enrollment.CompletedAt
        };
    }

    public static Enrollment ToEnrollment(this CreateEnrollmentRequest request, Guid studentId, Guid? subscriptionId = null)
    {
        return new Enrollment
        {
            EnrollmentId = Guid.NewGuid(),
            StudentId = studentId,
            CourseId = request.CourseId,
            Status = EnrollmentStatus.Enrolled,
            ProgressPct = 0,
            EnrolledAt = DateTime.UtcNow,
            ActivatedAt = null,
            SubscriptionId = subscriptionId
        };
    }

    public static EnrollmentDetailDto ToEnrollmentDetailDto(
        this Enrollment enrollment,
        List<Section> sections,
        List<LessonProgress> lessonProgresses)
    {
        var sectionProgressList = new List<SectionProgressDto>();

        foreach (var section in sections.OrderBy(s => s.OrderNumber))
        {
            var lessons = section.Lessons.OrderBy(l => l.OrderNumber).ToList();
            var lessonProgressDtos = new List<LessonProgressDto>();

            foreach (var lesson in lessons)
            {
                var progress = lessonProgresses.FirstOrDefault(lp => lp.LessonId == lesson.LessonId);
                lessonProgressDtos.Add(new LessonProgressDto
                {
                    LessonId = lesson.LessonId,
                    Title = lesson.Title,
                    OrderNumber = lesson.OrderNumber,
                    Status = progress?.Status.ToString() ?? "NotStarted",
                    IsFreePreview = lesson.IsFreePreview,
                    LastAccessedAt = progress?.LastAccessedAt,
                    CompletedAt = progress?.CompletedAt
                });
            }

            var completedCount = lessonProgressDtos.Count(l => l.Status == "Completed");

            sectionProgressList.Add(new SectionProgressDto
            {
                SectionId = section.SectionId,
                Title = section.Title,
                OrderNumber = section.OrderNumber,
                TotalLessons = lessons.Count,
                CompletedLessons = completedCount,
                Lessons = lessonProgressDtos
            });
        }

        var totalLessons = sections.Sum(s => s.Lessons.Count);
        var completedLessons = lessonProgresses.Count(lp => lp.Status == LessonProgressStatus.Completed);

        return new EnrollmentDetailDto
        {
            EnrollmentId = enrollment.EnrollmentId,
            CourseId = enrollment.CourseId,
            CourseTitle = enrollment.Course?.Title ?? string.Empty,
            CourseDescription = enrollment.Course?.Description,
            Status = enrollment.Status.ToString(),
            ProgressPct = enrollment.ProgressPct,
            EnrolledAt = enrollment.EnrolledAt,
            CompletedAt = enrollment.CompletedAt,
            TotalLessons = totalLessons,
            CompletedLessons = completedLessons,
            Sections = sectionProgressList
        };
    }
}
