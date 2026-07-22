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
[Route("api/jockey-profiles")]
[Authorize]
public class JockeyProfilesController : ControllerBase
{
    private readonly IJockeyProfileService _service;

    public JockeyProfilesController(IJockeyProfileService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lấy danh sách Jockey (để Admin quản lý hoặc Horse Owner tìm kiếm Jockey thuê).
    /// </summary>
    [HttpGet]
    [AllowAnonymous] // Bất kỳ ai (Spectator/Owner) cũng có thể xem danh sách Nài ngựa
    public async Task<ActionResult<ApiResponse<PagedResponse<JockeyProfileDto>>>> GetAll(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetAllJockeysAsync(page, pageSize);
        return Ok(ApiResponse<PagedResponse<JockeyProfileDto>>.Ok(result));
    }

    /// <summary>
    /// Xem chi tiết profile 1 Jockey.
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<JockeyProfileDto>>> GetById(int id)
    {
        var result = await _service.GetJockeyByIdAsync(id);
        return Ok(ApiResponse<JockeyProfileDto>.Ok(result));
    }

    /// <summary>
    /// Jockey xem profile của chính mình.
    /// </summary>
    [HttpGet("me")]
    [AuthorizeRoles(UserRole.Jockey)]
    public async Task<ActionResult<ApiResponse<JockeyProfileDto>>> GetMyProfile()
    {
        var result = await _service.GetMyJockeyProfileAsync(User.GetUserId());
        return Ok(ApiResponse<JockeyProfileDto>.Ok(result));
    }

    /// <summary>
    /// Jockey cập nhật thông tin cá nhân.
    /// </summary>
    [HttpPut("me")]
    [AuthorizeRoles(UserRole.Jockey)]
    public async Task<ActionResult<ApiResponse<JockeyProfileDto>>> UpdateMyProfile([FromBody] UpdateJockeyProfileDto dto)
    {
        var result = await _service.UpdateMyJockeyProfileAsync(User.GetUserId(), dto);
        return Ok(ApiResponse<JockeyProfileDto>.Ok(result, "Jockey profile updated successfully."));
    }
}