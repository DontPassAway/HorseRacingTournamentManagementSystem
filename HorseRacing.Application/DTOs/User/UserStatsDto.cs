namespace HorseRacing.Application.DTOs.Auth;

public class UserStatsDto
{
    public int Total { get; set; }
    public int Admin { get; set; }
    public int Owner { get; set; }
    public int Jockey { get; set; }
    public int Referee { get; set; }
    public int Spectator { get; set; }
}