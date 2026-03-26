using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.AchievementDTOs;

public class UnlockResultDto
{
    [JsonPropertyName("new_achievements")]
    public List<NewlyUnlockedAchievementDto> NewAchievements { get; set; } = new();

    public int TotalXPAwarded { get; set; }
}
