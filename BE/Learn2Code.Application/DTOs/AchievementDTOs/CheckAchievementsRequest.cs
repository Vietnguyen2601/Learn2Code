using System.ComponentModel.DataAnnotations;

namespace Learn2Code.Application.DTOs.AchievementDTOs;

public class CheckAchievementsRequest
{
    /// <summary>
    /// Loại điều kiện cần kiểm tra.
    /// Hợp lệ: "lessons_completed", "exercises_passed", "streak_days",
    ///         "total_xp", "level_reached", "course_completed"
    /// </summary>
    [Required]
    public string ConditionType { get; set; } = string.Empty;
}
