using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.TestCaseDTOs.TestCaseRequests;

public class CreateTestCaseRequest
{
    [JsonPropertyName("text_input")]
    public string? TextInput { get; set; }

    [Required]
    [JsonPropertyName("expected_output")]
    public string ExpectedOutput { get; set; } = string.Empty;

    [JsonPropertyName("is_hidden")]
    public bool IsHidden { get; set; } = false;

    [Range(0.1, 100)]
    [JsonPropertyName("weight")]
    public decimal Weight { get; set; } = 1;
}
