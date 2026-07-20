namespace HorseRacing.Application.DTOs.RaceResults;

public record CreateRaceResultDto(
    int RaceId,
    int RegistrationId,
    int Position,
    TimeSpan? FinishTime,
    bool Disqualified,
    string? DisqualificationReason
);

public record UpdateRaceResultDto(
    int Position,
    TimeSpan? FinishTime,
    bool Disqualified,
    string? DisqualificationReason
);

public class RaceResultDto
{
    public int Id { get; set; }
    public int RaceId { get; set; }
    public string RaceName { get; set; }
    public int RegistrationId { get; set; }
    public int HorseId { get; set; }
    public string HorseName { get; set; }
    public string? JockeyName { get; set; }
    public int Position { get; set; }
    public TimeSpan? FinishTime { get; set; }
    public bool Disqualified { get; set; }
    public string? DisqualificationReason { get; set; }
    public decimal? PrizeMoney { get; set; }
    public bool IsConfirmed { get; set; }
}
