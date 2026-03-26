using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.GamificationDTOs;

/// <summary>
/// Thông tin một ngày trong lịch sử streak (dùng cho StreakHistory)
/// </summary>
public class DailyStreakHistoryItemDto
{
    [JsonPropertyName("streak_date")]
    public DateOnly StreakDate { get; set; }

    [JsonPropertyName("xp_earned")]
    public int XPEarned { get; set; }

    [JsonPropertyName("current_day_count")]
    public int CurrentDayCount { get; set; }
}
