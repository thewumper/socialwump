namespace wumpapi.game;
/// <summary>
/// Constants used throughout the program 
/// </summary>
public static class Constants
{
    public const int ItemSlots = 5;
    public const int MinimumPlayers = 2;
    public static readonly TimeSpan TimeBetweenGames = TimeSpan.FromMinutes(0.1);
    public const float ConnectionExpiresSeconds = 60;
}