using Learn2Code.Application.Base;
using Learn2Code.Application.Constants;
using Learn2Code.Application.DTOs.AchievementDTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learn2Code.Application.Services;

public class AchievementService : IAchievementService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AchievementService> _logger;

    public AchievementService(IUnitOfWork unitOfWork, ILogger<AchievementService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Public queries
    // ─────────────────────────────────────────────────────────────────────────

    public async Task<ServiceResult<List<AchievementDto>>> GetAllAchievementsAsync()
    {
        var achievements = await _unitOfWork.Repository<Achievement>()
            .GetAllQueryable()
            .Where(a => !a.IsHidden)
            .OrderBy(a => a.ConditionType)
            .ThenBy(a => a.ConditionValue)
            .ToListAsync();

        return ServiceResult<List<AchievementDto>>.Ok(achievements.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<List<UserAchievementDto>>> GetUserAchievementsAsync(Guid userId)
    {
        var userAchievements = await _unitOfWork.Repository<UserAchievement>()
            .GetAllQueryable()
            .Where(ua => ua.UserId == userId)
            .Include(ua => ua.Achievement)
            .OrderByDescending(ua => ua.UnlockedAt)
            .ToListAsync();

        var dtos = userAchievements.Select(ua => new UserAchievementDto
        {
            UserAchievementId = ua.UserAchievementId,
            AchievementId     = ua.AchievementId,
            Name              = ua.Achievement.Name,
            Description       = ua.Achievement.Description,
            IconUrl           = ua.Achievement.IconUrl,
            XPReward          = ua.Achievement.XPReward,
            UnlockedAt        = ua.UnlockedAt
        }).ToList();

        return ServiceResult<List<UserAchievementDto>>.Ok(dtos);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Unlock logic
    // ─────────────────────────────────────────────────────────────────────────

    public async Task<ServiceResult<UnlockResultDto>> CheckAndUnlockAsync(Guid userId, string conditionType)
    {
        try
        {
            // ── 1. Load UserXP (cần cho điều kiện XP / Level / Streak) ──────────
            var userXp = await _unitOfWork.Repository<UserXP>()
                .GetAsync(x => x.UserId == userId);

            if (userXp == null)
                return ServiceResult<UnlockResultDto>.Ok(new UnlockResultDto());

            // ── 2. Load achievement chưa unlock, lọc theo conditionType ─────────
            var alreadyUnlockedIds = await _unitOfWork.Repository<UserAchievement>()
                .GetAllQueryable()
                .Where(ua => ua.UserId == userId)
                .Select(ua => ua.AchievementId)
                .ToListAsync();

            var pending = await _unitOfWork.Repository<Achievement>()
                .GetAllQueryable()
                .Where(a => !alreadyUnlockedIds.Contains(a.AchievementId)
                            && a.ConditionType == conditionType
                            && a.ConditionValue != null)
                .ToListAsync();

            if (!pending.Any())
                return ServiceResult<UnlockResultDto>.Ok(new UnlockResultDto());

            // ── 3. Lấy count một lần duy nhất (1 DB round-trip) ─────────────────
            int? count = conditionType switch
            {
                GamificationConstants.AchievementConditionLessons =>
                    await _unitOfWork.Repository<LessonProgress>()
                        .GetAllQueryable()
                        .CountAsync(lp => lp.StudentId == userId
                                          && lp.Status == LessonProgressStatus.Completed),

                GamificationConstants.AchievementConditionExercises =>
                    await _unitOfWork.Repository<ExerciseProgress>()
                        .GetAllQueryable()
                        .CountAsync(ep => ep.StudentId == userId && ep.IsPassed),

                GamificationConstants.AchievementConditionCourses =>
                    await _unitOfWork.Repository<Enrollment>()
                        .GetAllQueryable()
                        .CountAsync(e => e.StudentId == userId
                                         && e.Status == EnrollmentStatus.Completed),

                // XP / Level / Streak: đọc trực tiếp từ userXp, không cần count
                _ => null
            };

            // ── 4. Đánh giá từng achievement, chuẩn bị unlock ───────────────────
            var unlocked = new List<Achievement>();

            foreach (var achievement in pending)
            {
                var conditionMet = achievement.ConditionType switch
                {
                    GamificationConstants.AchievementConditionXP =>
                        userXp.TotalXP >= achievement.ConditionValue,
                    GamificationConstants.AchievementConditionLevel =>
                        userXp.CurrentLevel >= achievement.ConditionValue,
                    GamificationConstants.AchievementConditionStreak =>
                        userXp.CurrentStreak >= achievement.ConditionValue,
                    GamificationConstants.AchievementConditionLessons =>
                        count >= achievement.ConditionValue,
                    GamificationConstants.AchievementConditionExercises =>
                        count >= achievement.ConditionValue,
                    GamificationConstants.AchievementConditionCourses =>
                        count >= achievement.ConditionValue,
                    _ => false
                };

                if (conditionMet != true) continue;

                _unitOfWork.Repository<UserAchievement>().PrepareCreate(new UserAchievement
                {
                    UserAchievementId = Guid.NewGuid(),
                    UserId            = userId,
                    AchievementId     = achievement.AchievementId,
                    UnlockedAt        = DateTime.UtcNow
                });

                // Cộng bonus XP cho thành tựu
                if (achievement.XPReward > 0)
                    userXp.TotalXP += achievement.XPReward;

                unlocked.Add(achievement);
            }

            // ── 5. Persist ───────────────────────────────────────────────────────
            if (unlocked.Any())
            {
                userXp.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<UserXP>().PrepareUpdate(userXp);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "User {UserId} unlocked {Count} achievement(s): {Names}",
                    userId, unlocked.Count,
                    string.Join(", ", unlocked.Select(a => a.Name)));
            }

            var resultDto = new UnlockResultDto
            {
                NewAchievements = unlocked.Select(a => new NewlyUnlockedAchievementDto
                {
                    Name     = a.Name,
                    IconUrl  = a.IconUrl,
                    XPReward = a.XPReward
                }).ToList(),
                TotalXPAwarded = unlocked.Sum(a => a.XPReward)
            };

            var message = unlocked.Any() ? "Unlocked new achievements" : "No new achievements";
            return ServiceResult<UnlockResultDto>.Ok(resultDto, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "CheckAndUnlockAsync failed for user {UserId}, conditionType {ConditionType}",
                userId, conditionType);
            return ServiceResult<UnlockResultDto>.Error("UNLOCK_FAILED", "Failed to check achievements", 500);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Admin
    // ─────────────────────────────────────────────────────────────────────────

    public async Task<ServiceResult<List<AchievementDto>>> GetAllAchievementsAdminAsync()
    {
        var achievements = await _unitOfWork.Repository<Achievement>()
            .GetAllQueryable()
            .OrderBy(a => a.ConditionType)
            .ThenBy(a => a.ConditionValue)
            .ToListAsync();

        return ServiceResult<List<AchievementDto>>.Ok(achievements.Select(ToDto).ToList());
    }

    public async Task<ServiceResult<AchievementDto>> CreateAchievementAsync(CreateAchievementRequest request)
    {
        var nameExists = await _unitOfWork.Repository<Achievement>()
            .GetAllQueryable()
            .AnyAsync(a => a.Name == request.Name);

        if (nameExists)
            return ServiceResult<AchievementDto>.Error(
                "DUPLICATE_NAME", $"Achievement '{request.Name}' đã tồn tại", 409);

        var achievement = new Achievement
        {
            AchievementId  = Guid.NewGuid(),
            Name           = request.Name,
            Description    = request.Description,
            IconUrl        = request.IconUrl,
            XPReward       = request.XPReward,
            ConditionType  = request.ConditionType,
            ConditionValue = request.ConditionValue,
            IsHidden       = request.IsHidden,
            CreatedAt      = DateTime.UtcNow,
            UpdatedAt      = DateTime.UtcNow
        };

        _unitOfWork.Repository<Achievement>().PrepareCreate(achievement);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<AchievementDto>.Created(ToDto(achievement), "Achievement created");
    }

    public async Task<ServiceResult<AchievementDto>> UpdateAchievementAsync(Guid id, UpdateAchievementRequest request)
    {
        var achievement = await _unitOfWork.Repository<Achievement>()
            .GetAsync(a => a.AchievementId == id);

        if (achievement == null)
            return ServiceResult<AchievementDto>.NotFound("Achievement not found");

        // Kiểm tra tên trùng chỉ khi tên bị thay đổi
        if (request.Name != null && request.Name != achievement.Name)
        {
            var nameExists = await _unitOfWork.Repository<Achievement>()
                .GetAllQueryable()
                .AnyAsync(a => a.Name == request.Name && a.AchievementId != id);

            if (nameExists)
                return ServiceResult<AchievementDto>.Error(
                    "DUPLICATE_NAME", $"Tên '{request.Name}' đã được sử dụng", 409);

            achievement.Name = request.Name;
        }

        if (request.Description   != null)  achievement.Description   = request.Description;
        if (request.IconUrl       != null)  achievement.IconUrl       = request.IconUrl;
        if (request.XPReward      .HasValue) achievement.XPReward     = request.XPReward.Value;
        if (request.ConditionType != null)  achievement.ConditionType = request.ConditionType;
        if (request.ConditionValue.HasValue) achievement.ConditionValue = request.ConditionValue;
        if (request.IsHidden      .HasValue) achievement.IsHidden     = request.IsHidden.Value;

        achievement.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<Achievement>().PrepareUpdate(achievement);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult<AchievementDto>.Ok(ToDto(achievement), "Achievement updated");
    }

    public async Task<ServiceResult> DeleteAchievementAsync(Guid id)
    {
        var achievement = await _unitOfWork.Repository<Achievement>()
            .GetAsync(a => a.AchievementId == id);

        if (achievement == null)
            return ServiceResult.NotFound("Achievement not found");

        _unitOfWork.Repository<Achievement>().PrepareRemove(achievement);
        await _unitOfWork.SaveChangesAsync();

        return ServiceResult.Ok("Achievement deleted");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Helper
    // ─────────────────────────────────────────────────────────────────────────

    private static AchievementDto ToDto(Achievement a) => new()
    {
        AchievementId  = a.AchievementId,
        Name           = a.Name,
        Description    = a.Description,
        IconUrl        = a.IconUrl,
        XPReward       = a.XPReward,
        ConditionType  = a.ConditionType,
        ConditionValue = a.ConditionValue,
        IsHidden       = a.IsHidden,
        CreatedAt      = a.CreatedAt,
        UpdatedAt      = a.UpdatedAt
    };
}
