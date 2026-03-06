using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs;

public class TestCaseDto
{
    [JsonPropertyName("testcase_id")]
    public Guid TestCaseId { get; set; }

    [JsonPropertyName("exercise_id")]
    public Guid ExerciseId { get; set; }

    [JsonPropertyName("expected_output")]
    public string ExpectedOutput { get; set; } = string.Empty;

    [JsonPropertyName("is_hidden")]
    public bool IsHidden { get; set; }

    [JsonPropertyName("weight")]
    public decimal Weight { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

public class CreateTestCaseRequest
{
    [Required]
    [JsonPropertyName("expected_output")]
    public string ExpectedOutput { get; set; } = string.Empty;

    [JsonPropertyName("is_hidden")]
    public bool IsHidden { get; set; } = false;

    [Range(0.1, 100)]
    [JsonPropertyName("weight")]
    public decimal Weight { get; set; } = 1;
}

public class UpdateTestCaseRequest
{
    [JsonPropertyName("expected_output")]
    public string? ExpectedOutput { get; set; }

    [JsonPropertyName("is_hidden")]
    public bool? IsHidden { get; set; }

    [Range(0.1, 100)]
    [JsonPropertyName("weight")]
    public decimal? Weight { get; set; }
}
