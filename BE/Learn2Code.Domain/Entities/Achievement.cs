using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Learn2Code.Domain.Entities;

/// <summary>
/// Định nghĩa một thành tựu (badge) trong hệ thống gamification.
/// Admin tạo sẵn, hệ thống tự unlock cho user khi đủ điều kiện.
/// </summary>
[Table("achievements")]
public class Achievement
{
    [Key]
    [Column("achievement_id")]
    public Guid AchievementId { get; set; }

    /// <summary>Tên thành tựu, phải unique. Ví dụ: "First Lesson", "Python Master"</summary>
    [Required]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    /// <summary>Đường dẫn icon/badge hiển thị trên UI</summary>
    [Column("icon_url")]
    public string? IconUrl { get; set; }

    /// <summary>Số XP thưởng khi unlock thành tựu này</summary>
    [Column("xp_reward")]
    public int XPReward { get; set; } = 0;

    /// <summary>
    /// Loại điều kiện để unlock.
    /// Các giá trị: "lesson_completed", "course_completed", "streak_days",
    ///              "exercise_passed", "quiz_perfect"
    /// </summary>
    [Column("condition_type")]
    public string? ConditionType { get; set; }

    /// <summary>
    /// Ngưỡng số lượng cần đạt để unlock.
    /// Ví dụ: condition_type="streak_days", condition_value=7 → học 7 ngày liên tiếp
    /// </summary>
    [Column("condition_value")]
    public int? ConditionValue { get; set; }

    /// <summary>True = achievement ẩn, không hiển thị trong danh sách cho đến khi unlock</summary>
    [Column("is_hidden")]
    public bool IsHidden { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
}
