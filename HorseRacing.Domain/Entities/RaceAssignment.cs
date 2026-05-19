namespace HorseRacing.Domain.Entities;

public class RaceAssignment
{
    public int Id { get; set; }
    public int RaceId { get; set; }
    public int RefereeUserId { get; set; }
    public string? Notes { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Race Race { get; set; } = null!;
    public User RefereeUser { get; set; } = null!;
}
