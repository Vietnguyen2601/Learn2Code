using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.TestCaseDTOs.TestCaseRequests;

public class UpdateTestCaseRequest
{
    [JsonPropertyName("text_input")]
    public string? TextInput { get; set; }

    [JsonPropertyName("expected_output")]
    public string? ExpectedOutput { get; set; }
    /// <summary>
    /// Hàm main/validator function dành riêng cho test case này.
    /// </summary>
    [JsonPropertyName("validator_main")]
    public string? ValidatorMain { get; set; }
    [JsonPropertyName("is_hidden")]
    public bool? IsHidden { get; set; }

    [Range(0.1, 100)]
    [JsonPropertyName("weight")]
    public decimal? Weight { get; set; }
}
