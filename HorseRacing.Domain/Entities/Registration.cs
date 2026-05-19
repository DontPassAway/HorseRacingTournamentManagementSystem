using HorseRacing.Domain.Enums;

namespace HorseRacing.Domain.Entities;

public class Registration
{
    public int Id { get; set; }
    public int RaceId { get; set; }
    public int HorseId { get; set; }
    public int HorseOwnerId { get; set; }
    public int? JockeyId { get; set; } // assigned jockey profile ID
    public int LaneNumber { get; set; } // starting lane
    public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;
    public string? RejectionReason { get; set; }
    public bool JockeyConfirmed { get; set; } = false;
    public bool OwnerConfirmed { get; set; } = false;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Race Race { get; set; } = null!;
    public Horse Horse { get; set; } = null!;
    public HorseOwner HorseOwner { get; set; } = null!;
    public JockeyProfile? Jockey { get; set; }
    public RaceResult? RaceResult { get; set; }
}
