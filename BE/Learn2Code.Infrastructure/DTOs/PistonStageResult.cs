using System.Text.Json.Serialization;

namespace Learn2Code.Infrastructure.DTOs;

public class PistonStageResult
{
    [JsonPropertyName("stdout")]
    public string? Stdout { get; set; }

    [JsonPropertyName("stderr")]
    public string? Stderr { get; set; }

    [JsonPropertyName("output")]
    public string? Output { get; set; }

    [JsonPropertyName("code")]
    public int? Code { get; set; }

    [JsonPropertyName("signal")]
    public string? Signal { get; set; }
}
