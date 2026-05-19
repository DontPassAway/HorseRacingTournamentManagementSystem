using HorseRacing.API.Extensions;
using HorseRacing.API.Filters;
using HorseRacing.Application.DTOs.Horses;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Enums;
using HorseRacing.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HorseRacing.API.Controllers;

[ApiController]
[Route("api/horses")]
[Authorize]
public class HorsesController : ControllerBase
{
    private readonly IHorseService _horseService;
    public HorsesController(IHorseService horseService) => _horseService = horseService;

    [HttpPost]
    [AuthorizeRoles(UserRole.HorseOwner)]
    public async Task<ActionResult<ApiResponse<HorseDto>>> Create([FromBody] CreateHorseDto dto)
    {
        var result = await _horseService.CreateHorseAsync(User.GetUserId(), dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<HorseDto>.Ok(result, "Horse created."));
    }

    [HttpGet]
    [AuthorizeRoles(UserRole.Admin, UserRole.Referee)]
    public async Task<ActionResult<ApiResponse<PagedResponse<HorseDto>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        => Ok(ApiResponse<PagedResponse<HorseDto>>.Ok(await _horseService.GetAllHorsesAsync(page, pageSize)));

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<HorseDto>>> GetById(int id)
        => Ok(ApiResponse<HorseDto>.Ok(await _horseService.GetHorseByIdAsync(id)));

    [HttpGet("my-horses")]
    [AuthorizeRoles(UserRole.HorseOwner)]
    public async Task<ActionResult<ApiResponse<PagedResponse<HorseDto>>>> GetMyHorses(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        => Ok(ApiResponse<PagedResponse<HorseDto>>.Ok(await _horseService.GetMyHorsesAsync(User.GetUserId(), page, pageSize)));

    [HttpPut("{id:int}")]
    [AuthorizeRoles(UserRole.HorseOwner)]
    public async Task<ActionResult<ApiResponse<HorseDto>>> Update(int id, [FromBody] UpdateHorseDto dto)
        => Ok(ApiResponse<HorseDto>.Ok(await _horseService.UpdateHorseAsync(id, User.GetUserId(), dto)));

    [HttpDelete("{id:int}")]
    [AuthorizeRoles(UserRole.HorseOwner, UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _horseService.DeleteHorseAsync(id, User.GetUserId());
        return Ok(ApiResponse<object>.Ok(null!, "Horse deleted."));
    }
}
