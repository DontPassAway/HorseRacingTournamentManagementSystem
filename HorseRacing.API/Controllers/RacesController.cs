using HorseRacing.API.Extensions;
using HorseRacing.API.Filters;
using HorseRacing.Application.DTOs.Races;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Enums;
using HorseRacing.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Enums;
using HorseRacing.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HorseRacing.API.Controllers;

[ApiController]
[Route("api/races")]
public class RacesController : ControllerBase
{
    private readonly IRaceService _service;
    public RacesController(IRaceService service) => _service = service;

    [HttpPost]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<RaceDto>>> Create([FromBody] CreateRaceDto dto)
    {
        var result = await _service.CreateRaceAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<RaceDto>.Ok(result));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PagedResponse<RaceDto>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] int? tournamentId = null)
        => Ok(ApiResponse<PagedResponse<RaceDto>>.Ok(await _service.GetAllRacesAsync(page, pageSize, tournamentId)));

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<RaceDto>>> GetById(int id)
        => Ok(ApiResponse<RaceDto>.Ok(await _service.GetRaceByIdAsync(id)));

    [HttpPut("{id:int}")]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<RaceDto>>> Update(int id, [FromBody] UpdateRaceDto dto)
        => Ok(ApiResponse<RaceDto>.Ok(await _service.UpdateRaceAsync(id, dto)));

    [HttpPut("{id:int}/status")]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<RaceDto>>> UpdateStatus(int id, [FromBody] UpdateRaceStatusDto dto)
        => Ok(ApiResponse<RaceDto>.Ok(await _service.UpdateStatusAsync(id, dto.Status)));

    [HttpDelete("{id:int}")]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _service.DeleteRaceAsync(id);
        return Ok(ApiResponse<object>.Ok(null!, "Race deleted."));
    }

    [HttpPost("{raceId:int}/check-horse")]
    [AuthorizeRoles(UserRole.Referee)]
    public async Task<ActionResult<ApiResponse<HorseCheckResultDto>>> CheckHorse(
        int raceId, [FromBody] CheckHorseDto dto)
        => Ok(ApiResponse<HorseCheckResultDto>.Ok(
            await _service.CheckHorseEligibilityAsync(raceId, User.GetUserId(), dto),
            "Horse eligibility checked."));
}

public record UpdateRaceStatusDto(RaceStatus Status);
