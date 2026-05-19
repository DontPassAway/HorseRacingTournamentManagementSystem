namespace HorseRacing.Domain.Entities;

public class HorseOwner
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? LicenseNumber { get; set; }
    public string? Address { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Horse> Horses { get; set; } = new List<Horse>();
}
