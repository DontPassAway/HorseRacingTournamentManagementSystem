using HorseRacing.API.Extensions;
using HorseRacing.Application.DTOs.Notifications;
using HorseRacing.Application.Interfaces.Services;
using HorseRacing.Shared.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HorseRacing.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;
    public NotificationsController(INotificationService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<NotificationDto>>>> GetMy()
        => Ok(ApiResponse<List<NotificationDto>>.Ok(
            await _service.GetMyNotificationsAsync(User.GetUserId())));
}
