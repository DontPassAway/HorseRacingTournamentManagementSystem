namespace HorseRacing.Shared.Constants;

public static class AppConstants
{
    public const string AdminRole = "Admin";
    public const string HorseOwnerRole = "HorseOwner";
    public const string JockeyRole = "Jockey";
    public const string RefereeRole = "Referee";
    public const string SpectatorRole = "Spectator";

    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    public const string TokenClaimUserId = "userId";
    public const string TokenClaimRole = "role";
    public const string TokenClaimEmail = "email";
}
