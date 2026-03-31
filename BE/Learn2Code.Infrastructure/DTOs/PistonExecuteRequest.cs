using System.Text.Json.Serialization;

namespace Learn2Code.Infrastructure.DTOs;

public class PistonExecuteRequest
{
    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = "*";

    [JsonPropertyName("files")]
    public List<PistonFileDto> Files { get; set; } = new();

    [JsonPropertyName("stdin")]
    public string Stdin { get; set; } = string.Empty;

    [JsonPropertyName("args")]
    public List<string> Args { get; set; } = new();

    [JsonPropertyName("run_timeout")]
    public int? RunTimeout { get; set; }

    [JsonPropertyName("compile_timeout")]
    public int? CompileTimeout { get; set; }
}
