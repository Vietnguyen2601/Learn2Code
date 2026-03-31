using System.Text.Json.Serialization;

namespace Learn2Code.Infrastructure.DTOs;

public class PistonFileDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}
