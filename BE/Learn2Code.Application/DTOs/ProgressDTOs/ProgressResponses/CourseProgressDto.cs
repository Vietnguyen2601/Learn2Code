using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.ProgressDTOs.ProgressResponses;

public class CourseProgressDto
{
    [JsonPropertyName("enrollment_status")]
    public string EnrollmentStatus { get; set; } = string.Empty;

    [JsonPropertyName("progress_pct")]
    public decimal ProgressPct { get; set; }

    [JsonPropertyName("sections")]
    public List<SectionProgressDto> Sections { get; set; } = new();
}
