using HorseRacing.Application.DTOs.Notifications;

namespace HorseRacing.Application.Interfaces.Services;

public interface INotificationService
{
    Task<List<NotificationDto>> GetMyNotificationsAsync(int userId);
}
