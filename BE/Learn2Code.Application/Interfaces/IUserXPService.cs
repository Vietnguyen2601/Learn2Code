using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.GamificationDTOs;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Application.Interfaces;

public interface IUserXPService
{
    /// <summary>
    /// Get XP info for a specific user
    /// </summary>
    Task<ServiceResult<UserXPDto>> GetUserXPAsync(Guid userId);

    /// <summary>
    /// Award XP to user based on event
    /// Handles level calculation and streak bonuses internally
    /// </summary>
    Task<ServiceResult<AwardXPResult>> AwardXPAsync(Guid userId, XPEventType eventType);
}
