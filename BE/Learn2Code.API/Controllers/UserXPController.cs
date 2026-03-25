using Learn2Code.Application.DTOs.GamificationDTOs;
using Learn2Code.Application.Interfaces;
using Learn2Code.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/userxp")]
public class UserXPController : ControllerBase
{
    private readonly IUserXPService _userXPService;
    private readonly ILogger<UserXPController> _logger;

    public UserXPController(IUserXPService userXPService, ILogger<UserXPController> logger)
    {
        _userXPService = userXPService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's XP and level information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserXPDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCurrentUserXP()
    {
        var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? Guid.Empty.ToString());
        if (userId == Guid.Empty)
            return Unauthorized("User ID not found in token");

        var result = await _userXPService.GetUserXPAsync(userId);
        if (!result.Success)
            return NotFound(result.Message);

        return Ok(result);
    }

    /// <summary>
    /// Admin: Get XP and level information for a specific user
    /// </summary>
    [HttpGet("admin/{userId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserXPDto), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUserXPByAdmin(Guid userId)
    {
        var result = await _userXPService.GetUserXPAsync(userId);
        if (!result.Success)
            return NotFound(result.Message);

        return Ok(result);
    }
}
