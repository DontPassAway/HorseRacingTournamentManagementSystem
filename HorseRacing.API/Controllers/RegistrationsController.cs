using HorseRacing.API.Extensions;
using HorseRacing.API.Filters;
using HorseRacing.Application.DTOs.Registrations;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Enums;
using HorseRacing.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HorseRacing.API.Controllers;

[ApiController]
[Route("api/registrations")]
[Authorize]
public class RegistrationsController : ControllerBase
{
    private readonly IRegistrationService _service;
    public RegistrationsController(IRegistrationService service) => _service = service;

    [HttpGet]
    [AuthorizeRoles(UserRole.Admin, UserRole.Referee)]
    public async Task<ActionResult<ApiResponse<PagedResponse<RegistrationDto>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        => Ok(ApiResponse<PagedResponse<RegistrationDto>>.Ok(await _service.GetAllRegistrationsAsync(page, pageSize)));

    [HttpPost]
    [AuthorizeRoles(UserRole.HorseOwner)]
    public async Task<ActionResult<ApiResponse<RegistrationDto>>> Register([FromBody] CreateRegistrationDto dto)
    {
        var result = await _service.RegisterHorseAsync(User.GetUserId(), dto);
        return Ok(ApiResponse<RegistrationDto>.Ok(result, "Registered for race."));
    }

    [HttpGet("my-registrations")]
    [AuthorizeRoles(UserRole.HorseOwner)]
    public async Task<ActionResult<ApiResponse<PagedResponse<RegistrationDto>>>> GetMy(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        => Ok(ApiResponse<PagedResponse<RegistrationDto>>.Ok(await _service.GetMyRegistrationsAsync(User.GetUserId(), page, pageSize)));

    [HttpGet("race/{raceId:int}")]
    [AuthorizeRoles(UserRole.Admin, UserRole.Referee)]
    public async Task<ActionResult<ApiResponse<PagedResponse<RegistrationDto>>>> GetByRace(
        int raceId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        => Ok(ApiResponse<PagedResponse<RegistrationDto>>.Ok(await _service.GetRegistrationsByRaceAsync(raceId, page, pageSize)));

    [HttpPut("{id:int}/approve")]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<RegistrationDto>>> Approve(int id, [FromBody] ApproveRegistrationDto dto)
        => Ok(ApiResponse<RegistrationDto>.Ok(await _service.ApproveRegistrationAsync(id, dto)));

    [HttpPut("{id:int}/reject")]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<RegistrationDto>>> Reject(int id, [FromBody] RejectRegistrationDto dto)
        => Ok(ApiResponse<RegistrationDto>.Ok(await _service.RejectRegistrationAsync(id, dto)));

    [HttpPut("{id:int}/confirm-jockey")]
    [AuthorizeRoles(UserRole.HorseOwner)]
    public async Task<ActionResult<ApiResponse<RegistrationDto>>> ConfirmJockey(int id, [FromBody] ConfirmJockeyDto dto)
        => Ok(ApiResponse<RegistrationDto>.Ok(await _service.ConfirmJockeyAsync(id, User.GetUserId(), dto)));

    [HttpPut("{id:int}/withdraw")]
    [AuthorizeRoles(UserRole.HorseOwner)]
    public async Task<ActionResult<ApiResponse<object>>> Withdraw(int id)
    {
        await _service.WithdrawRegistrationAsync(id, User.GetUserId());
        return Ok(ApiResponse<object>.Ok(null!, "Withdrawn."));
    }

    // Trong file HorseRacing.API/Controllers/RegistrationsController.cs

    [HttpGet("my-rides")]
    [AuthorizeRoles(UserRole.Jockey)]
    public async Task<ActionResult<ApiResponse<PagedResponse<RegistrationDto>>>> GetMyRides(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        // Gọi Service (Bạn cần thêm logic Where(r => r.Jockey.UserId == currentUserId) trong RegistrationService)
        var result = await _service.GetMyRidesAsync(User.GetUserId(), page, pageSize);
        return Ok(ApiResponse<PagedResponse<RegistrationDto>>.Ok(result));
    }
}
