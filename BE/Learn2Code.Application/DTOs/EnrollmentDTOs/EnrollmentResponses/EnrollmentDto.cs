using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.EnrollmentDTOs.EnrollmentResponses;

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
