using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.CategoryDTOs.CategoryResponses;

public class CategoryDto
{
    [JsonPropertyName("category_id")]
    public Guid CategoryId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
