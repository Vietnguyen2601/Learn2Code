using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.AdminDTOs.AdminResponses;

public class DashboardDto
{
    [JsonPropertyName("total_users")]
    public int TotalUsers { get; set; }

    [JsonPropertyName("total_courses")]
    public int TotalCourses { get; set; }

    [JsonPropertyName("total_enrollments")]
    public int TotalEnrollments { get; set; }

    [JsonPropertyName("total_revenue")]
    public decimal TotalRevenue { get; set; }

    [JsonPropertyName("active_subscriptions")]
    public int ActiveSubscriptions { get; set; }
}

public class RevenueByMonthDto
{
    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("month")]
    public int Month { get; set; }

    [JsonPropertyName("revenue")]
    public decimal Revenue { get; set; }

    [JsonPropertyName("payment_count")]
    public int PaymentCount { get; set; }
}

public class EnrollmentByCourseDashboardDto
{
    [JsonPropertyName("course_id")]
    public Guid CourseId { get; set; }

    [JsonPropertyName("course_title")]
    public string CourseTitle { get; set; } = string.Empty;

    [JsonPropertyName("enrollment_count")]
    public int EnrollmentCount { get; set; }

    [JsonPropertyName("completed_count")]
    public int CompletedCount { get; set; }
}
