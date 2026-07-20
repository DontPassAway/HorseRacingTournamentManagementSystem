namespace HorseRacing.Application.DTOs.RaceAssignments;

public record CreateRaceAssignmentDto(
    int RaceId,
    int RefereeUserId,
    string? Notes
);

public class RaceAssignmentDto
{
    public int Id { get; set; }
    public int RaceId { get; set; }
    public string RaceName { get; set; }
    public int RefereeUserId { get; set; }
    public string RefereeName { get; set; }
    public string? Notes { get; set; }
    public DateTime AssignedAt { get; set; }
}
