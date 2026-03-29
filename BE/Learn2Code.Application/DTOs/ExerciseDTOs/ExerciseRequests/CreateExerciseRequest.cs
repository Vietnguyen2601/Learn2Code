using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.ExerciseDTOs.ExerciseRequests;

public class CreateExerciseRequest
{
    [Required]
    [JsonPropertyName("exercise_type")]
    [RegularExpression("^(Reading|FreeCode|GradedCode)$", ErrorMessage = "Exercise type must be one of: Reading, FreeCode, GradedCode")]
    public string ExerciseType { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("narrative")]
    public string Narrative { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("starter_code")]
    public string? StarterCode { get; set; }

    [JsonPropertyName("solution_code")]
    public string? SolutionCode { get; set; }

    [JsonPropertyName("instruction")]
    public string? Instruction { get; set; }

    [JsonPropertyName("hint")]
    public string? Hint { get; set; }

    /// <summary>
    /// Code test harness do admin viết (chỉ dành cho GradedCode).
    /// In ra từng dòng "PASS" hoặc "FAIL[:message]" cho mỗi test case.
    /// </summary>
    [JsonPropertyName("solution_validator")]
    public string? SolutionValidator { get; set; }

    [JsonPropertyName("order_number")]
    [Range(1, int.MaxValue)]
    public int? OrderNumber { get; set; }
}
