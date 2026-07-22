using HorseRacing.API.Extensions;
using HorseRacing.API.Filters;
using HorseRacing.Application.DTOs.RaceResults;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Enums;
using HorseRacing.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HorseRacing.API.Controllers;

[ApiController]
[Route("api/race-results")]
[Authorize]
public class RaceResultsController : ControllerBase
{
    private readonly IRaceResultService _service;
    public RaceResultsController(IRaceResultService service) => _service = service;

    [HttpPost]
    [AuthorizeRoles(UserRole.Admin, UserRole.Referee)]
    public async Task<ActionResult<ApiResponse<RaceResultDto>>> Create([FromBody] CreateRaceResultDto dto)
        => Ok(ApiResponse<RaceResultDto>.Ok(await _service.CreateResultAsync(dto)));

    [HttpGet("race/{raceId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<RaceResultDto>>>> GetByRace(int raceId)
        => Ok(ApiResponse<List<RaceResultDto>>.Ok(await _service.GetResultsByRaceAsync(raceId)));

    [HttpPut("{id:int}")]
    [AuthorizeRoles(UserRole.Admin, UserRole.Referee)]
    public async Task<ActionResult<ApiResponse<RaceResultDto>>> Update(int id, [FromBody] UpdateRaceResultDto dto)
        => Ok(ApiResponse<RaceResultDto>.Ok(await _service.UpdateResultAsync(id, dto)));

    [HttpPut("{id:int}/confirm")]
    [AuthorizeRoles(UserRole.Admin, UserRole.Referee)]
    public async Task<ActionResult<ApiResponse<RaceResultDto>>> Confirm(int id)
        => Ok(ApiResponse<RaceResultDto>.Ok(await _service.ConfirmResultAsync(id, User.GetUserId())));
}
