using HorseRacing.API.Extensions;
using HorseRacing.API.Filters;
using HorseRacing.Application.DTOs.RaceAssignments;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Enums;
using HorseRacing.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HorseRacing.API.Controllers;

[ApiController]
[Route("api/race-assignments")]
[Authorize]
public class RaceAssignmentsController : ControllerBase
{
    private readonly IRaceAssignmentService _service;
    public RaceAssignmentsController(IRaceAssignmentService service) => _service = service;

    [HttpPost]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<RaceAssignmentDto>>> Assign([FromBody] CreateRaceAssignmentDto dto)
        => Ok(ApiResponse<RaceAssignmentDto>.Ok(await _service.AssignRefereeAsync(dto)));

    [HttpGet("my-assignments")]
    [AuthorizeRoles(UserRole.Jockey)]
    public async Task<ActionResult<ApiResponse<PagedResponse<RaceAssignmentDto>>>> GetMy(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        => Ok(ApiResponse<PagedResponse<RaceAssignmentDto>>.Ok(await _service.GetMyAssignmentsAsync(User.GetUserId(), page, pageSize)));

    [HttpDelete("{id:int}")]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _service.DeleteAssignmentAsync(id);
        return Ok(ApiResponse<object>.Ok(null!, "Assignment removed."));
    }
}
