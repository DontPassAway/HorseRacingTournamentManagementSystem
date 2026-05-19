using HorseRacing.Domain.Enums;

namespace HorseRacing.Domain.Entities;

public class JockeyInvitation
{
    public int Id { get; set; }
    public int HorseId { get; set; }
    public int HorseOwnerId { get; set; }
    public int JockeyId { get; set; } // JockeyProfile.Id
    public int? RaceId { get; set; } // which race this is for
    public JockeyInvitationStatus Status { get; set; } = JockeyInvitationStatus.Pending;
    public string? Message { get; set; }
    public string? ResponseMessage { get; set; }
    public DateTime InvitedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }

    // Navigation properties
    public Horse Horse { get; set; } = null!;
    public HorseOwner HorseOwner { get; set; } = null!;
    public JockeyProfile Jockey { get; set; } = null!;
    public Race? Race { get; set; }
}
