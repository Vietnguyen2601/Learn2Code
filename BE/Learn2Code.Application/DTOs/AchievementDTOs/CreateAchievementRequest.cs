using System.ComponentModel.DataAnnotations;

namespace Learn2Code.Application.DTOs.AchievementDTOs;

public class CreateAchievementRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public int XPReward { get; set; } = 0;

    /// <summary>
    /// Các giá trị hợp lệ: "lessons_completed", "exercises_passed",
    /// "streak_days", "total_xp", "level_reached", "course_completed"
    /// </summary>
    public string? ConditionType { get; set; }

    public int? ConditionValue { get; set; }
    public bool IsHidden { get; set; } = false;
}
