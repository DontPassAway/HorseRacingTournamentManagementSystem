using HorseRacing.Domain.Enums;

namespace HorseRacing.Domain.Entities;

public class Bet
{
    public int Id { get; set; }
    public int SpectatorUserId { get; set; }
    public int RaceId { get; set; }
    public int PredictedHorseId { get; set; } // predicted winning horse
    public int PredictedPosition { get; set; } // predicted finishing position (1st, 2nd, etc.)
    public BetStatus Status { get; set; } = BetStatus.Pending;
    public string? Notes { get; set; }
    public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }

    // Navigation properties
    public User SpectatorUser { get; set; } = null!;
    public Race Race { get; set; } = null!;
    public Horse PredictedHorse { get; set; } = null!;
}
