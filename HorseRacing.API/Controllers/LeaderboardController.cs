using HorseRacing.API.Filters;
using HorseRacing.Application.DTOs.Leaderboard;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HorseRacing.API.Controllers;

[ApiController]
[Route("api/leaderboard")]
public class LeaderboardController : ControllerBase
{
    private readonly ILeaderboardService _service;
    public LeaderboardController(ILeaderboardService service) => _service = service;

    [HttpGet("jockeys")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<JockeyLeaderboardDto>>>> GetJockeys(
        [FromQuery] int top = 20)
        => Ok(ApiResponse<List<JockeyLeaderboardDto>>.Ok(await _service.GetJockeyLeaderboardAsync(top)));

    [HttpGet("horses")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<HorseLeaderboardDto>>>> GetHorses(
        [FromQuery] int top = 20)
        => Ok(ApiResponse<List<HorseLeaderboardDto>>.Ok(await _service.GetHorseLeaderboardAsync(top)));
}
