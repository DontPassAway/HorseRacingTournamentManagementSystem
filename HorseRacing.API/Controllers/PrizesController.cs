using HorseRacing.API.Filters;
using HorseRacing.Application.DTOs.Prizes;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Enums;
using HorseRacing.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HorseRacing.API.Controllers;

[ApiController]
[Route("api/prizes")]
public class PrizesController : ControllerBase
{
    private readonly IPrizeService _service;
    public PrizesController(IPrizeService service) => _service = service;

    [HttpPost]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<PrizeDto>>> Create([FromBody] CreatePrizeDto dto)
        => Ok(ApiResponse<PrizeDto>.Ok(await _service.CreatePrizeAsync(dto)));

    [HttpGet("tournament/{tournamentId:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<List<PrizeDto>>>> GetByTournament(int tournamentId)
        => Ok(ApiResponse<List<PrizeDto>>.Ok(await _service.GetPrizesByTournamentAsync(tournamentId)));

    [HttpPut("{id:int}")]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<PrizeDto>>> Update(int id, [FromBody] UpdatePrizeDto dto)
        => Ok(ApiResponse<PrizeDto>.Ok(await _service.UpdatePrizeAsync(id, dto)));

    [HttpDelete("{id:int}")]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _service.DeletePrizeAsync(id);
        return Ok(ApiResponse<object>.Ok(null!, "Prize deleted."));
    }
}
