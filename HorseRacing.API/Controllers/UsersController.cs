using HorseRacing.API.Filters;
using HorseRacing.Application.DTOs.Auth;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Domain.Enums;
using HorseRacing.Shared.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace HorseRacing.API.Controllers;

[ApiController]
[Route("api/users")]
[AuthorizeRoles(UserRole.Admin)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponse<UserProfileDto>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] bool? isActive = null)
    {
        var result = await _userService.GetAllUsersAsync(page, pageSize, search, isActive);
        return Ok(ApiResponse<PagedResponse<UserProfileDto>>.Ok(result));
    }

    // Endpoint mới để trả về thống kê
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<UserStatsDto>>> GetStats()
    {
        var result = await _userService.GetUserStatsAsync();
        return Ok(ApiResponse<UserStatsDto>.Ok(result));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetById(int id)
        => Ok(ApiResponse<UserProfileDto>.Ok(await _userService.GetUserByIdAsync(id)));

    [HttpPut("{id:int}/role")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> UpdateRole(int id, [FromBody] UpdateRoleDto dto)
        => Ok(ApiResponse<UserProfileDto>.Ok(await _userService.UpdateUserRoleAsync(id, dto.Role)));

    [HttpPut("{id:int}/toggle-active")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> ToggleActive(int id)
        => Ok(ApiResponse<UserProfileDto>.Ok(await _userService.ToggleUserActiveAsync(id)));

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _userService.DeleteUserAsync(id);
        return Ok(ApiResponse<object>.Ok(null!, "User deleted."));
    }
}

public record UpdateRoleDto(UserRole Role);