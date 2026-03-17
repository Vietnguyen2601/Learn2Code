using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.TestCaseDTOs.TestCaseRequests;

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
