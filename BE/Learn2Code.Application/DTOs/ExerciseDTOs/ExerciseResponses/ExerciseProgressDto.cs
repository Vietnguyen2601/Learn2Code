using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.ExerciseDTOs.ExerciseResponses;

public class ExerciseProgressDto
{
    [JsonPropertyName("exprogress_id")]
    public Guid ExProgressId { get; set; }

    [JsonPropertyName("student_id")]
    public Guid StudentId { get; set; }

    [JsonPropertyName("exercise_id")]
    public Guid ExerciseId { get; set; }

    [JsonPropertyName("is_completed")]
    public bool IsCompleted { get; set; }

    [JsonPropertyName("is_passed")]
    public bool IsPassed { get; set; }

    [JsonPropertyName("last_code")]
    public string? LastCode { get; set; }

    [JsonPropertyName("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("stdout")]
    public string? Stdout { get; set; }

    [JsonPropertyName("stderr")]
    public string? Stderr { get; set; }

    [JsonPropertyName("output")]
    public string? Output { get; set; }

    [JsonPropertyName("exit_code")]
    public int? ExitCode { get; set; }

    [JsonPropertyName("runtime_ms")]
    public int? RuntimeMs { get; set; }

    [JsonPropertyName("compile_stdout")]
    public string? CompileStdout { get; set; }

    [JsonPropertyName("compile_stderr")]
    public string? CompileStderr { get; set; }

    [JsonPropertyName("compile_output")]
    public string? CompileOutput { get; set; }

    [JsonPropertyName("testcase_results")]
    public List<ExerciseTestCaseResultDto>? TestCaseResults { get; set; }
}
