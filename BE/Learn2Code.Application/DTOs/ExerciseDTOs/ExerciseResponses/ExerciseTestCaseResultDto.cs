using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.ExerciseDTOs.ExerciseResponses;

public class ExerciseTestCaseResultDto
{
    [JsonPropertyName("testcase_id")]
    public Guid TestCaseId { get; set; }

    [JsonPropertyName("is_passed")]
    public bool IsPassed { get; set; }

    [JsonPropertyName("actual_output")]
    public string? ActualOutput { get; set; }
}
