using System.Text.Json.Serialization;

namespace Learn2Code.Infrastructure.DTOs;

public class PistonFileDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

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
