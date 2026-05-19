namespace HorseRacing.Application.DTOs.Prizes;

public record CreatePrizeDto(
    int TournamentId,
    int Position,
    decimal Amount,
    string? Description
);

public record UpdatePrizeDto(
    int Position,
    decimal Amount,
    string? Description
);

public record PrizeDto(
    int Id,
    int TournamentId,
    string TournamentName,
    int Position,
    decimal Amount,
    string? Description
);
