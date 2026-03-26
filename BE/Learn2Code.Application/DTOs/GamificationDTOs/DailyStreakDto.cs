using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.GamificationDTOs;

/// <summary>
/// Thông tin streak hiện tại của user (lấy từ UserXP + DailyStreak)
/// </summary>
public class DailyStreakDto
{
    [JsonPropertyName("current_streak")]
    public int CurrentStreak { get; set; }

    [JsonPropertyName("longest_streak")]
    public int LongestStreak { get; set; }

    [JsonPropertyName("today_xp")]
    public int TodayXP { get; set; }

    [JsonPropertyName("streak_history")]
    public List<DailyStreakHistoryItemDto> StreakHistory { get; set; } = new();

    [JsonPropertyName("xp_available_today")]
    public bool XPAvailableToday { get; set; }
}
