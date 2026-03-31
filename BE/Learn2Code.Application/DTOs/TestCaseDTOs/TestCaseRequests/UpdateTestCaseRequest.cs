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
    /// Hàm main riêng của testcase. Null để xóa.
    /// </summary>
    [JsonPropertyName("main_code")]
    public string? MainCode { get; set; }
    [JsonPropertyName("is_hidden")]
    public bool? IsHidden { get; set; }

    [Range(0.1, 100)]
    [JsonPropertyName("weight")]
    public decimal? Weight { get; set; }
}
