using HorseRacing.API.Filters;
using HorseRacing.Application.DTOs.RefereeProfiles;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Enums;
using HorseRacing.Shared.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace HorseRacing.API.Controllers;

[ApiController]
[Route("api/referee-profiles")]
[AuthorizeRoles(UserRole.Admin)]
public class RefereeProfilesController : ControllerBase
{
    private readonly IRefereeProfileService _service;
    public RefereeProfilesController(IRefereeProfileService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<RefereeProfileDto>>>> GetAll()
        => Ok(ApiResponse<List<RefereeProfileDto>>.Ok(await _service.GetAllRefereesAsync()));
}
