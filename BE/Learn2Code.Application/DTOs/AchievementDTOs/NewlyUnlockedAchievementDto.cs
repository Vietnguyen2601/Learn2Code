using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.AchievementDTOs;

/// <summary>
/// Shape khớp với yêu cầu response khi unlock:
/// { "name": "First Steps", "icon_url": "...", "xp_reward": 100 }
/// </summary>
public class NewlyUnlockedAchievementDto
{
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("icon_url")]
    public string? IconUrl { get; set; }

    [JsonPropertyName("xp_reward")]
    public int XPReward { get; set; }
}
