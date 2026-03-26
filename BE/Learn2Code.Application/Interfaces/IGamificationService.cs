using Learn2Code.Application.DTOs.GamificationDTOs;
using Learn2Code.Domain.Enums;

namespace Learn2Code.Application.Interfaces;

public interface IGamificationService
{
    /// <summary>
    /// Awards XP and evaluates streaks / achievements for the given event.
    /// Safe to call fire-and-forget style; exceptions are swallowed internally
    /// so that a gamification failure never breaks the main user flow.
    /// </summary>
    Task<GamificationResult> ProcessEventAsync(Guid userId, XPEventType eventType);
}
