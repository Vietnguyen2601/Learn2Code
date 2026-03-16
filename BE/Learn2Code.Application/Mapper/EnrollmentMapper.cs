using Learn2Code.Application.DTOs;
using Learn2Code.Domain.Entities;

namespace Learn2Code.Application.Mapper;

public static class EnrollmentMapper
{
    public static EnrollmentDto ToDto(this Enrollment enrollment)
    {
        return new EnrollmentDto
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            CourseId = enrollment.CourseId,
            Status = enrollment.Status.ToString(),
            ProgressPct = enrollment.ProgressPct,
            EnrolledAt = enrollment.EnrolledAt,
            ActivatedAt = enrollment.ActivatedAt,
            CompletedAt = enrollment.CompletedAt,
            SubscriptionId = enrollment.SubscriptionId
        };
    }

    public static EnrollmentDetailDto ToDetailDto(this Enrollment enrollment)
    {
        return new EnrollmentDetailDto
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            CourseId = enrollment.CourseId,
            Status = enrollment.Status.ToString(),
            ProgressPct = enrollment.ProgressPct,
            EnrolledAt = enrollment.EnrolledAt,
            ActivatedAt = enrollment.ActivatedAt,
            CompletedAt = enrollment.CompletedAt,
            SubscriptionId = enrollment.SubscriptionId,
            CourseTitle = enrollment.Course?.Title ?? string.Empty,
            CourseDifficulty = enrollment.Course?.Difficulty?.ToString(),
            StudentName = enrollment.Student?.Name,
            StudentEmail = enrollment.Student?.Email ?? string.Empty
        };
    }

    public static Enrollment ToEntity(this CreateEnrollmentRequest request, Guid studentId, Guid? subscriptionId)
    {
        return new Enrollment
        {
            EnrollmentId = Guid.NewGuid(),
            StudentId = studentId,
            CourseId = request.CourseId,
            Status = Domain.Enums.EnrollmentStatus.Enrolled,
            ProgressPct = 0,
            EnrolledAt = DateTime.UtcNow,
            SubscriptionId = subscriptionId
        };
    }
}
