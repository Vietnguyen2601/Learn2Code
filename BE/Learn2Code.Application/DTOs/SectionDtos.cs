using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs;

public class SectionDto
{
    [JsonPropertyName("section_id")]
    public Guid SectionId { get; set; }

    [JsonPropertyName("course_id")]
    public Guid CourseId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }

    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

public class CreateSectionRequest
{
    [Required]
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [Required]
    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }
}

public class UpdateSectionRequest
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("order_number")]
    public int? OrderNumber { get; set; }

    [JsonPropertyName("is_active")]
    public bool? IsActive { get; set; }
}

public class ReorderSectionsRequest
{
    [Required]
    [JsonPropertyName("section_orders")]
    public List<SectionOrderItem> SectionOrders { get; set; } = new();
}

public class SectionOrderItem
{
    [Required]
    [JsonPropertyName("section_id")]
    public Guid SectionId { get; set; }

    [Required]
    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }
}
