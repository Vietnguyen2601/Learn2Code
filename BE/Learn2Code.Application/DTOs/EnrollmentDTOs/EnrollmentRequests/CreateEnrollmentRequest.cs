using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.EnrollmentDTOs.EnrollmentRequests;

public class CreateEnrollmentRequest
{
    [Required]
    [JsonPropertyName("course_id")]
    public Guid CourseId { get; set; }
}
