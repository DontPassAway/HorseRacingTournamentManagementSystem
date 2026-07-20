using HorseRacing.Domain.Enums;

namespace HorseRacing.Application.DTOs.Bets;

public record CreateBetDto(
    int RaceId,
    int PredictedHorseId,
    int PredictedPosition,
    string? Notes
);

public class BetDto
{
    public int Id { get; set; }
    public int SpectatorUserId { get; set; }
    public string SpectatorName { get; set; }
    public int RaceId { get; set; }
    public string RaceName { get; set; }
    public int PredictedHorseId { get; set; }
    public string PredictedHorseName { get; set; }
    public int PredictedPosition { get; set; }
    public BetStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime PlacedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
