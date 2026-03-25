using Learn2Code.Application.Base;
using Learn2Code.Application.Constants;
using Learn2Code.Application.DTOs.GamificationDTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Application.Mapper;
using Learn2Code.Domain.Entities;
using Learn2Code.Domain.Enums;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Learn2Code.Application.Services;

public class UserXPService : IUserXPService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserXPService> _logger;

    public UserXPService(IUnitOfWork unitOfWork, ILogger<UserXPService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ServiceResult<UserXPDto>> GetUserXPAsync(Guid userId)
    {
        try
        {
            var userXp = await _unitOfWork.Repository<UserXP>()
                .GetAsync(x => x.UserId == userId);

            if (userXp == null)
                return ServiceResult<UserXPDto>.NotFound("User XP not found");

            var dto = new UserXPDto
            {
                UserId = userXp.UserId,
                TotalXP = userXp.TotalXP,
                CurrentLevel = userXp.CurrentLevel,
                XPToNext = userXp.XPToNext,
                CurrentStreak = userXp.CurrentStreak,
                LongestStreak = userXp.LongestStreak,
                UpdatedAt = userXp.UpdatedAt
            };

            return ServiceResult<UserXPDto>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting UserXP for user {UserId}", userId);
            return ServiceResult<UserXPDto>.Error("GET_XP_FAILED", "Failed to retrieve user XP", 500);
        }
    }

    public async Task<ServiceResult<AwardXPResult>> AwardXPAsync(Guid userId, XPEventType eventType)
    {
        try
        {
            var baseXP = GamificationConstants.XPRewards.GetValueOrDefault(eventType, 0);
            var result = new AwardXPResult { XPGained = baseXP };

            // ── 1. Get or create UserXP record ──────────────────────────────
            var userXp = await _unitOfWork.Repository<UserXP>()
                .GetAsync(x => x.UserId == userId);

            var isNew = userXp == null;
            if (isNew)
            {
                userXp = new UserXP
                {
                    UserXPId = Guid.NewGuid(),
                    UserId = userId,
                    TotalXP = 0,
                    CurrentLevel = 1,
                    XPToNext = GamificationConstants.BaseXpPerLevel,
                    CurrentStreak = 0,
                    LongestStreak = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }

            var prevLevel = userXp.CurrentLevel;

            // ── 2. Daily streak handling ─────────────────────────────────────
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var streakBonus = 0;

            var todayStreak = await _unitOfWork.Repository<DailyStreak>()
                .GetAsync(ds => ds.UserId == userId && ds.StreakDate == today);

            if (todayStreak == null)
            {
                // First activity today → check if yesterday has a record to continue streak
                var yesterday = today.AddDays(-1);
                var hadActivityYesterday = await _unitOfWork.Repository<DailyStreak>()
                    .GetAllQueryable()
                    .AnyAsync(ds => ds.UserId == userId && ds.StreakDate == yesterday);

                userXp.CurrentStreak = hadActivityYesterday ? userXp.CurrentStreak + 1 : 1;
                if (userXp.CurrentStreak > userXp.LongestStreak)
                    userXp.LongestStreak = userXp.CurrentStreak;

                // Award streak bonus XP
                streakBonus = GamificationConstants.GetStreakBonus(userXp.CurrentStreak);
                result.StreakBonus = streakBonus;

                // Create DailyStreak record
                var newStreak = new DailyStreak
                {
                    StreakId = Guid.NewGuid(),
                    UserId = userId,
                    StreakDate = today,
                    XPEarned = baseXP + streakBonus,
                    CreatedAt = DateTime.UtcNow
                };
                _unitOfWork.Repository<DailyStreak>().PrepareCreate(newStreak);
            }
            else
            {
                // Already logged today; just accumulate XP on the existing record
                todayStreak.XPEarned += baseXP;
                _unitOfWork.Repository<DailyStreak>().PrepareUpdate(todayStreak);
            }

            result.StreakDays = userXp.CurrentStreak;

            // ── 3. Award XP ──────────────────────────────────────────────────
            userXp.TotalXP += baseXP + streakBonus;
            userXp.CurrentLevel = GamificationConstants.CalculateLevel(userXp.TotalXP);
            userXp.XPToNext = GamificationConstants.XpToNextLevel(userXp.TotalXP);
            userXp.UpdatedAt = DateTime.UtcNow;

            if (isNew)
                _unitOfWork.Repository<UserXP>().PrepareCreate(userXp);
            else
                _unitOfWork.Repository<UserXP>().PrepareUpdate(userXp);

            result.NewTotalXP = userXp.TotalXP;
            result.CurrentLevel = userXp.CurrentLevel;
            result.LevelUp = userXp.CurrentLevel > prevLevel;
            result.XpToNext = userXp.XPToNext;

            // ── 4. Persist ───────────────────────────────────────────────────
            await _unitOfWork.SaveChangesAsync();

            return ServiceResult<AwardXPResult>.Ok(result, "XP awarded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error awarding XP to user {UserId} for event {EventType}", userId, eventType);
            return ServiceResult<AwardXPResult>.Error("AWARD_XP_FAILED", "Failed to award XP", 500);
        }
    }
}
