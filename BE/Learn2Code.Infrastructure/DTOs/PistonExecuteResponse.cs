using System.Text.Json.Serialization;

namespace Learn2Code.Infrastructure.DTOs;

public class PistonExecuteResponse
{
    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("run")]
    public PistonStageResult? Run { get; set; }

    [JsonPropertyName("compile")]
    public PistonStageResult? Compile { get; set; }
}
