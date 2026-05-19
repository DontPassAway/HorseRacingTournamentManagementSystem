using HorseRacing.API.Extensions;
using HorseRacing.API.Filters;
using HorseRacing.Application.DTOs.Tournaments;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Enums;
using HorseRacing.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HorseRacing.API.Controllers;

[ApiController]
[Route("api/tournaments")]
public class TournamentsController : ControllerBase
{
    private readonly ITournamentService _service;
    public TournamentsController(ITournamentService service) => _service = service;

    [HttpPost]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<TournamentDto>>> Create([FromBody] CreateTournamentDto dto)
    {
        var result = await _service.CreateTournamentAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<TournamentDto>.Ok(result));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PagedResponse<TournamentDto>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        => Ok(ApiResponse<PagedResponse<TournamentDto>>.Ok(await _service.GetAllTournamentsAsync(page, pageSize)));

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<TournamentDto>>> GetById(int id)
        => Ok(ApiResponse<TournamentDto>.Ok(await _service.GetTournamentByIdAsync(id)));

    [HttpPut("{id:int}")]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<TournamentDto>>> Update(int id, [FromBody] UpdateTournamentDto dto)
        => Ok(ApiResponse<TournamentDto>.Ok(await _service.UpdateTournamentAsync(id, dto)));

    [HttpPut("{id:int}/status")]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<TournamentDto>>> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        => Ok(ApiResponse<TournamentDto>.Ok(await _service.UpdateStatusAsync(id, dto.Status)));

    [HttpDelete("{id:int}")]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _service.DeleteTournamentAsync(id);
        return Ok(ApiResponse<object>.Ok(null!, "Tournament deleted."));
    }
}

public record UpdateStatusDto(TournamentStatus Status);
