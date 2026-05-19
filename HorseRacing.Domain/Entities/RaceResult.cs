namespace HorseRacing.Domain.Entities;

public class RaceResult
{
    public int Id { get; set; }
    public int RaceId { get; set; }
    public int RegistrationId { get; set; }
    public int Position { get; set; } // finishing position
    public TimeSpan? FinishTime { get; set; }
    public bool Disqualified { get; set; } = false;
    public string? DisqualificationReason { get; set; }
    public decimal? PrizeMoney { get; set; }
    public bool IsConfirmed { get; set; } = false;
    public int? ConfirmedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Race Race { get; set; } = null!;
    public Registration Registration { get; set; } = null!;
    public User? ConfirmedBy { get; set; }
}
