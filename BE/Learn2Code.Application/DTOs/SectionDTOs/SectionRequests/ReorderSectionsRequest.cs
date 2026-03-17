using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.SectionDTOs.SectionRequests;

public class ReorderSectionsRequest
{
    [Required]
    [JsonPropertyName("section_orders")]
    public List<SectionOrderItem> SectionOrders { get; set; } = new();
}
