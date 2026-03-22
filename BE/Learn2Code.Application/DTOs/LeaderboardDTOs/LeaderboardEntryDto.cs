using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.LeaderboardDTOs;

public class LeaderboardEntryDto
{
    [JsonPropertyName("rank")]
    public int? Rank { get; set; }

    [JsonPropertyName("student_id")]
    public Guid StudentId { get; set; }

    [JsonPropertyName("student_name")]
    public string? StudentName { get; set; }

    [JsonPropertyName("total_score")]
    public decimal TotalScore { get; set; }
}
