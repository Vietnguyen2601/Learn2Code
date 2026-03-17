using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.SubscriptionDTOs.SubscriptionRequests;

public class CreateSubscriptionRequest
{
    [Required]
    [JsonPropertyName("package_id")]
    public Guid PackageId { get; set; }
}
