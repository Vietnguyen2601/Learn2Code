using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.GamificationDTOs;

namespace Learn2Code.Application.Interfaces;

public interface IDailyStreakService
{
    /// <summary>
    /// Lấy thông tin streak hiện tại của user (current, longest, history 30 ngày gần nhất)
    /// </summary>
    Task<ServiceResult<DailyStreakDto>> GetUserStreakAsync(Guid userId);

    /// <summary>
    /// Check-in daily streak sau khi user hoàn thành Exercise (GradedCode) hoặc SectionQuiz.
    /// Logic:
    ///   - Nếu đã check-in hôm nay → return early
    ///   - Nếu hôm qua có record → CurrentStreak += 1
    ///   - Nếu hôm qua ko có → CurrentStreak reset = 1
    ///   - Award XP thưởng theo streak
    ///   - Unlock achievement nếu đạt mốc (7, 30 ngày...)
    /// </summary>
    Task<ServiceResult<CheckInStreakResult>> CheckInAsync(Guid userId);
}
