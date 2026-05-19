using HorseRacing.API.Extensions;
using HorseRacing.API.Filters;
using HorseRacing.Application.DTOs.JockeyInvitations;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Enums;
using HorseRacing.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HorseRacing.API.Controllers;

[ApiController]
[Route("api/jockey-invitations")]
[Authorize]
public class JockeyInvitationsController : ControllerBase
{
    private readonly IJockeyInvitationService _service;
    public JockeyInvitationsController(IJockeyInvitationService service) => _service = service;

    [HttpPost]
    [AuthorizeRoles(UserRole.HorseOwner)]
    public async Task<ActionResult<ApiResponse<JockeyInvitationDto>>> Send([FromBody] CreateJockeyInvitationDto dto)
        => Ok(ApiResponse<JockeyInvitationDto>.Ok(await _service.SendInvitationAsync(User.GetUserId(), dto), "Invitation sent."));

    [HttpGet("received")]
    [AuthorizeRoles(UserRole.Jockey)]
    public async Task<ActionResult<ApiResponse<PagedResponse<JockeyInvitationDto>>>> GetReceived(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        => Ok(ApiResponse<PagedResponse<JockeyInvitationDto>>.Ok(await _service.GetReceivedInvitationsAsync(User.GetUserId(), page, pageSize)));

    [HttpGet("sent")]
    [AuthorizeRoles(UserRole.HorseOwner)]
    public async Task<ActionResult<ApiResponse<PagedResponse<JockeyInvitationDto>>>> GetSent(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        => Ok(ApiResponse<PagedResponse<JockeyInvitationDto>>.Ok(await _service.GetSentInvitationsAsync(User.GetUserId(), page, pageSize)));

    [HttpPut("{id:int}/accept")]
    [AuthorizeRoles(UserRole.Jockey)]
    public async Task<ActionResult<ApiResponse<JockeyInvitationDto>>> Accept(int id, [FromBody] RespondInvitationDto dto)
        => Ok(ApiResponse<JockeyInvitationDto>.Ok(await _service.AcceptInvitationAsync(id, User.GetUserId(), dto)));

    [HttpPut("{id:int}/decline")]
    [AuthorizeRoles(UserRole.Jockey)]
    public async Task<ActionResult<ApiResponse<JockeyInvitationDto>>> Decline(int id, [FromBody] RespondInvitationDto dto)
        => Ok(ApiResponse<JockeyInvitationDto>.Ok(await _service.DeclineInvitationAsync(id, User.GetUserId(), dto)));
}
