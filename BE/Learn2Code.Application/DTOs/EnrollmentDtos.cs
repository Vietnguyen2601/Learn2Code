using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs;

public class EnrollmentDto
{
    [JsonPropertyName("enrollment_id")]
    public Guid EnrollmentId { get; set; }

    [JsonPropertyName("student_id")]
    public Guid StudentId { get; set; }

    [JsonPropertyName("course_id")]
    public Guid CourseId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("progress_pct")]
    public decimal ProgressPct { get; set; }

    [JsonPropertyName("enrolled_at")]
    public DateTime EnrolledAt { get; set; }

    [JsonPropertyName("activated_at")]
    public DateTime? ActivatedAt { get; set; }

    [JsonPropertyName("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [JsonPropertyName("subscription_id")]
    public Guid? SubscriptionId { get; set; }
}

public class EnrollmentDetailDto : EnrollmentDto
{
    [JsonPropertyName("course_title")]
    public string CourseTitle { get; set; } = string.Empty;

    [JsonPropertyName("course_difficulty")]
    public string? CourseDifficulty { get; set; }

    [JsonPropertyName("student_name")]
    public string? StudentName { get; set; }

    [JsonPropertyName("student_email")]
    public string StudentEmail { get; set; } = string.Empty;
}

public class CreateEnrollmentRequest
{
    [Required]
    [JsonPropertyName("course_id")]
    public Guid CourseId { get; set; }
}
