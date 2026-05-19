using HorseRacing.Domain.Enums;

namespace HorseRacing.Application.DTOs.Bets;

public record CreateBetDto(
    int RaceId,
    int PredictedHorseId,
    int PredictedPosition,
    string? Notes
);

public record BetDto(
    int Id,
    int SpectatorUserId,
    string SpectatorName,
    int RaceId,
    string RaceName,
    int PredictedHorseId,
    string PredictedHorseName,
    int PredictedPosition,
    BetStatus Status,
    string? Notes,
    DateTime PlacedAt,
    DateTime? ResolvedAt
);
