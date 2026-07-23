using HorseRacing.API.Extensions;
using HorseRacing.API.Filters;
using HorseRacing.Application.DTOs.Bets;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Enums;
using HorseRacing.Infrastructure.Services;
using HorseRacing.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HorseRacing.API.Controllers;

[ApiController]
[Route("api/bets")]
[Authorize]
public class BetsController : ControllerBase
{
    private readonly IBetService _service;
    public BetsController(IBetService service) => _service = service;

    [HttpGet]
    [AuthorizeRoles(UserRole.Admin, UserRole.Referee)] 
    public async Task<ActionResult<ApiResponse<PagedResponse<BetDto>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 15)
    {
        var result = await _service.GetAllBetsAsync(page, pageSize);
        return Ok(ApiResponse<PagedResponse<BetDto>>.Ok(result));
    }

    [HttpPost]
    [AuthorizeRoles(UserRole.Spectator)]
    public async Task<ActionResult<ApiResponse<BetDto>>> Place([FromBody] CreateBetDto dto)
        => Ok(ApiResponse<BetDto>.Ok(await _service.PlaceBetAsync(User.GetUserId(), dto), "Bet placed."));

    [HttpGet("my-bets")]
    [AuthorizeRoles(UserRole.Spectator)]
    public async Task<ActionResult<ApiResponse<PagedResponse<BetDto>>>> GetMy(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        => Ok(ApiResponse<PagedResponse<BetDto>>.Ok(await _service.GetMyBetsAsync(User.GetUserId(), page, pageSize)));

    [HttpGet("race/{raceId:int}")]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<PagedResponse<BetDto>>>> GetByRace(
        int raceId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        => Ok(ApiResponse<PagedResponse<BetDto>>.Ok(await _service.GetBetsByRaceAsync(raceId, page, pageSize)));

    [HttpPost("race/{raceId:int}/resolve")]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Resolve(int raceId)
    {
        await _service.ResolveBetsForRaceAsync(raceId);
        return Ok(ApiResponse<object>.Ok(null!, "Bets resolved."));
    }

    [HttpGet("odds/race/{raceId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<BetOddsDto>>> GetOdds(int raceId)
        => Ok(ApiResponse<BetOddsDto>.Ok(await _service.GetOddsForRaceAsync(raceId)));
}
