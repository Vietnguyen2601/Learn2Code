using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.SectionDTOs.SectionRequests;

public class SectionOrderItem
{
    [Required]
    [JsonPropertyName("section_id")]
    public Guid SectionId { get; set; }

    [Required]
    [JsonPropertyName("order_number")]
    public int OrderNumber { get; set; }
}
