using HorseRacing.Domain.Enums;

namespace HorseRacing.Domain.Entities;

public class RefereeReport
{
    public int Id { get; set; }
    public int RaceId { get; set; }
    public int RefereeUserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool HasViolation { get; set; } = false;
    public ViolationType? ViolationType { get; set; }
    public string? ViolationDescription { get; set; }
    public int? ViolatingRegistrationId { get; set; } // which horse/registration violated
    public bool IsFinalized { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Race Race { get; set; } = null!;
    public User RefereeUser { get; set; } = null!;
    public Registration? ViolatingRegistration { get; set; }
}
