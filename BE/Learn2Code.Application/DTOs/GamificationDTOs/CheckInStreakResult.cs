using System.Text.Json.Serialization;

namespace Learn2Code.Application.DTOs.GamificationDTOs;

/// <summary>
/// Kết quả check-in daily streak (dùng cho POST /api/streaks/check-in)
/// </summary>
public class CheckInStreakResult
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("current_streak")]
    public int CurrentStreak { get; set; }

    [JsonPropertyName("xp_earned_today")]
    public int XPEarnedToday { get; set; }

    [JsonPropertyName("new_badge_unlocked")]
    public string? NewBadgeUnlocked { get; set; }

    [JsonPropertyName("total_xp_bonus_awarded")]
    public int TotalXPBonusAwarded { get; set; }
}
