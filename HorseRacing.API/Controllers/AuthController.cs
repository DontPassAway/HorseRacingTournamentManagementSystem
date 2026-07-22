using HorseRacing.API.Extensions;
using HorseRacing.Application.DTOs.Auth;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HorseRacing.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Đăng ký tài khoản mới (Admin, HorseOwner, Jockey, Spectator)
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "User registered successfully."));
    }

    /// <summary>
    /// Đăng nhập và lấy JWT Token
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Login successful."));
    }

    /// <summary>
    /// Lấy thông tin User đang đăng nhập hiện tại
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetMe()
    {
        // Sử dụng extension method GetUserId() từ Claims của HttpContext
        var userId = User.GetUserId();
        var result = await _authService.GetCurrentUserAsync(userId);

        return Ok(ApiResponse<UserProfileDto>.Ok(result));
    }
}