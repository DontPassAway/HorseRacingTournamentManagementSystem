using HorseRacing.Domain.Enums;

namespace HorseRacing.Domain.Entities;

public class Bet
{
    public int Id { get; set; }
    public int SpectatorUserId { get; set; }
    public int RaceId { get; set; }
    public int PredictedHorseId { get; set; }       // con ngựa dự đoán thắng
    public int PredictedPosition { get; set; }       // vị trí dự đoán về đích
    public decimal Amount { get; set; } = 0m;        // số tiền đặt cược
    public decimal OddsMultiplier { get; set; } = 1m; // hệ số nhân tại thời điểm đặt (snapshot)
    public decimal? Payout { get; set; }             // tiền thắng thực tế (null = chưa resolve)
    public BetStatus Status { get; set; } = BetStatus.Pending;
    public string? Notes { get; set; }
    public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }

    // Navigation properties
    public User SpectatorUser { get; set; } = null!;
    public Race Race { get; set; } = null!;
    public Horse PredictedHorse { get; set; } = null!;
}
