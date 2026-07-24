using HorseRacing.Domain.Enums;

namespace HorseRacing.Application.DTOs.Bets;

public record CreateBetDto(
    int RaceId,
    int PredictedHorseId,
    int PredictedPosition,
    decimal Amount,
    string? Notes
);

public class BetDto
{
    public int Id { get; set; }
    public int SpectatorUserId { get; set; }
    public string SpectatorName { get; set; } = string.Empty;
    public int RaceId { get; set; }
    public string RaceName { get; set; } = string.Empty;
    public int PredictedHorseId { get; set; }
    public string PredictedHorseName { get; set; } = string.Empty;
    public int PredictedPosition { get; set; }
    public decimal Amount { get; set; }
    public decimal OddsMultiplier { get; set; }
    public decimal? Payout { get; set; }
    public BetStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime PlacedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class HorseOddsDto
{
    public int HorseId { get; set; }
    public string HorseName { get; set; } = string.Empty;
    public int BetCount { get; set; }
    public decimal TotalAmountBet { get; set; }
    public decimal Percentage { get; set; }
    public decimal OddsMultiplier { get; set; }
}

public class BetOddsDto
{
    public int RaceId { get; set; }
    public string RaceName { get; set; } = string.Empty;
    public int TotalBets { get; set; }
    public decimal TotalPoolAmount { get; set; }
    public List<HorseOddsDto> Odds { get; set; } = new();
}

public class BetSummaryDto
{
    public int TotalBets { get; set; }
    public int TotalWon { get; set; }
    public int TotalLost { get; set; }
    public int TotalPending { get; set; }
    public decimal TotalAmountBet { get; set; }
    public decimal TotalPayout { get; set; }
    public decimal NetProfit { get; set; }
    public decimal WinRate { get; set; }
}

public class BetLeaderboardEntryDto
{
    public int Rank { get; set; }
    public int SpectatorUserId { get; set; }
    public string SpectatorName { get; set; } = string.Empty;
    public int TotalWins { get; set; }
    public int TotalBets { get; set; }
    public decimal TotalPayout { get; set; }
    public decimal TotalAmountBet { get; set; }
    public decimal NetProfit { get; set; }
}
