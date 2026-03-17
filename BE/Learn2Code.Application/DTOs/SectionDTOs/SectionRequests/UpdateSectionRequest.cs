using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.SectionDTOs.SectionRequests;

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
