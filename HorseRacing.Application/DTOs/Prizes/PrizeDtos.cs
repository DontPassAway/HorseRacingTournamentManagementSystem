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

public class PrizeDto
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public string TournamentName { get; set; }
    public int Position { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}
