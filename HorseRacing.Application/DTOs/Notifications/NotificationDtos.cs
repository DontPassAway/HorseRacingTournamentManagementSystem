namespace HorseRacing.Application.DTOs.Notifications;

public class NotificationDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;      // "JockeyInvitation" | "BetResult" | "RegistrationStatus"
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ReferenceId { get; set; }                  // ID của bản ghi liên quan
    public string? ReferenceType { get; set; }             // "JockeyInvitation" | "Bet" | "Registration"
}
