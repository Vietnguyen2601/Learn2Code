using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.AchievementDTOs;

namespace Learn2Code.Application.Interfaces;

public interface IAchievementService
{
    // ── Public ────────────────────────────────────────────────────────────────

    /// <summary>Trả về tất cả achievement không bị ẩn (catalogue công khai).</summary>
    Task<ServiceResult<List<AchievementDto>>> GetAllAchievementsAsync();

    /// <summary>Trả về danh sách achievement user đã unlock.</summary>
    Task<ServiceResult<List<UserAchievementDto>>> GetUserAchievementsAsync(Guid userId);

    /// <summary>
    /// Kiểm tra và unlock achievement theo conditionType.
    /// Gọi sau khi: hoàn thành bài học, khoá học, cập nhật streak, award XP.
    /// Safe để gọi as fire-and-forget — exception được log và nuốt trong service.
    /// </summary>
    Task<ServiceResult<UnlockResultDto>> CheckAndUnlockAsync(Guid userId, string conditionType);

    // ── Admin ─────────────────────────────────────────────────────────────────

    /// <summary>Admin: Lấy tất cả achievement kể cả ẩn.</summary>
    Task<ServiceResult<List<AchievementDto>>> GetAllAchievementsAdminAsync();

    /// <summary>Admin: Tạo achievement mới.</summary>
    Task<ServiceResult<AchievementDto>> CreateAchievementAsync(CreateAchievementRequest request);

    /// <summary>Admin: Cập nhật achievement.</summary>
    Task<ServiceResult<AchievementDto>> UpdateAchievementAsync(Guid id, UpdateAchievementRequest request);

    /// <summary>Admin: Xoá achievement.</summary>
    Task<ServiceResult> DeleteAchievementAsync(Guid id);
}
