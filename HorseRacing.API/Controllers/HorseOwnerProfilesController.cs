using HorseRacing.API.Extensions;
using HorseRacing.API.Filters;
using HorseRacing.Application.DTOs.Profiles;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Enums;
using HorseRacing.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HorseRacing.API.Controllers;

[ApiController]
[Route("api/owner-profiles")]
[Authorize]
public class HorseOwnerProfilesController : ControllerBase
{
    private readonly IHorseOwnerProfileService _service;

    public HorseOwnerProfilesController(IHorseOwnerProfileService service)
    {
        _service = service;
    }

    /// <summary>
    /// Admin lấy danh sách chủ ngựa
    /// </summary>
    [HttpGet]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<ActionResult<ApiResponse<PagedResponse<HorseOwnerProfileDto>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetAllOwnersAsync(page, pageSize);
        return Ok(ApiResponse<PagedResponse<HorseOwnerProfileDto>>.Ok(result));
    }

    /// <summary>
    /// Xem chi tiết một chủ ngựa (Ai cũng có thể xem để biết thông tin trại ngựa/chủ ngựa)
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<HorseOwnerProfileDto>>> GetById(int id)
    {
        var result = await _service.GetOwnerByIdAsync(id);
        return Ok(ApiResponse<HorseOwnerProfileDto>.Ok(result));
    }

    /// <summary>
    /// Xem profile của chính chủ ngựa đang đăng nhập
    /// </summary>
    [HttpGet("me")]
    [AuthorizeRoles(UserRole.HorseOwner)]
    public async Task<ActionResult<ApiResponse<HorseOwnerProfileDto>>> GetMyProfile()
    {
        var result = await _service.GetMyOwnerProfileAsync(User.GetUserId());
        return Ok(ApiResponse<HorseOwnerProfileDto>.Ok(result));
    }

    /// <summary>
    /// Chủ ngựa cập nhật thông tin cá nhân/trại ngựa của mình
    /// </summary>
    [HttpPut("me")]
    [AuthorizeRoles(UserRole.HorseOwner)]
    public async Task<ActionResult<ApiResponse<HorseOwnerProfileDto>>> UpdateMyProfile([FromBody] UpdateHorseOwnerProfileDto dto)
    {
        var result = await _service.UpdateMyOwnerProfileAsync(User.GetUserId(), dto);
        return Ok(ApiResponse<HorseOwnerProfileDto>.Ok(result, "Owner profile updated successfully."));
    }
}