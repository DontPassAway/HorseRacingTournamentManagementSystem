namespace HorseRacing.Domain.Entities;

public class Prize
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public int Position { get; set; } // 1st, 2nd, 3rd, etc.
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Tournament Tournament { get; set; } = null!;
}
