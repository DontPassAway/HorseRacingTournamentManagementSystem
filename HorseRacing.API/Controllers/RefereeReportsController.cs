using HorseRacing.API.Extensions;
using HorseRacing.API.Filters;
using HorseRacing.Application.DTOs.RefereeReports;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Enums;
using HorseRacing.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HorseRacing.API.Controllers;

[ApiController]
[Route("api/referee-reports")]
[Authorize]
public class RefereeReportsController : ControllerBase
{
    private readonly IRefereeReportService _service;
    public RefereeReportsController(IRefereeReportService service) => _service = service;

    [HttpPost]
    [AuthorizeRoles(UserRole.Referee)]
    public async Task<ActionResult<ApiResponse<RefereeReportDto>>> Create([FromBody] CreateRefereeReportDto dto)
        => Ok(ApiResponse<RefereeReportDto>.Ok(await _service.CreateReportAsync(User.GetUserId(), dto)));

    [HttpGet("race/{raceId:int}")]
    [AuthorizeRoles(UserRole.Admin, UserRole.Referee)]
    public async Task<ActionResult<ApiResponse<List<RefereeReportDto>>>> GetByRace(int raceId)
        => Ok(ApiResponse<List<RefereeReportDto>>.Ok(await _service.GetReportsByRaceAsync(raceId)));

    [HttpPut("{id:int}")]
    [AuthorizeRoles(UserRole.Referee)]
    public async Task<ActionResult<ApiResponse<RefereeReportDto>>> Update(int id, [FromBody] UpdateRefereeReportDto dto)
        => Ok(ApiResponse<RefereeReportDto>.Ok(await _service.UpdateReportAsync(id, User.GetUserId(), dto)));
}
