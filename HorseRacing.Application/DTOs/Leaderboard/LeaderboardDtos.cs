namespace HorseRacing.Application.DTOs.Leaderboard;

public class JockeyLeaderboardDto
{
    public int Rank { get; set; }
    public int JockeyId { get; set; }
    public int UserId { get; set; }
    public string JockeyName { get; set; } = string.Empty;
    public int TotalRaces { get; set; }
    public int TotalWins { get; set; }
    public decimal WinRate { get; set; }
}

public class HorseLeaderboardDto
{
    public int Rank { get; set; }
    public int HorseId { get; set; }
    public string HorseName { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string Breed { get; set; } = string.Empty;
    public int TotalRaces { get; set; }
    public int TotalWins { get; set; }
    public decimal WinRate { get; set; }
}
