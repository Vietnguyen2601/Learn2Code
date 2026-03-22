using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs.LeaderboardDTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/courses/{courseId}/leaderboard")]
public class LeaderboardController : ControllerBase
{
    private readonly ILeaderboardService _leaderboardService;

    public LeaderboardController(ILeaderboardService leaderboardService)
    {
        _leaderboardService = leaderboardService;
    }

    /// <summary>
    /// Get leaderboard for a course (top 50, public)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ServiceResult<List<LeaderboardEntryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ServiceResult<List<LeaderboardEntryDto>>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLeaderboard(Guid courseId, [FromQuery] int top = 50)
    {
        var result = await _leaderboardService.GetLeaderboardAsync(courseId, top);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
